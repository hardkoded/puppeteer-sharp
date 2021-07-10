using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Helpers.Json;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class NetworkManager
    {
        #region Private members

        private readonly CDPSession _client;

        private readonly ConcurrentDictionary<string, Request> _requestIdToRequest =
            new ConcurrentDictionary<string, Request>();

        private readonly ConcurrentDictionary<string, RequestWillBeSentPayload> _requestIdToRequestWillBeSentEvent =
            new ConcurrentDictionary<string, RequestWillBeSentPayload>();

        private readonly ConcurrentDictionary<string, FetchRequestPausedResponse> _requestIdToRequestPausedEvent =
            new ConcurrentDictionary<string, FetchRequestPausedResponse>();

        private readonly ILogger _logger;
        private Dictionary<string, string> _extraHTTPHeaders;
        private Credentials _credentials;
        private readonly ConcurrentSet<string> _attemptedAuthentications = new ConcurrentSet<string>();
        private bool _userRequestInterceptionEnabled;
        private bool _protocolRequestInterceptionEnabled;
        private readonly bool _ignoreHTTPSErrors;
        private bool _userCacheDisabled;
        private readonly InternalNetworkConditions _emulatedNetworkConditions = new InternalNetworkConditions
        {
            Offline = false,
            Upload = -1,
            Download = -1,
            Latency = 0,
        };
        #endregion

        internal NetworkManager(CDPSession client, bool ignoreHTTPSErrors, FrameManager frameManager)
        {
            FrameManager = frameManager;
            _client = client;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _client.MessageReceived += Client_MessageReceived;
            _logger = _client.Connection.LoggerFactory.CreateLogger<NetworkManager>();
        }

        #region Public Properties
        internal Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();

        internal event EventHandler<ResponseCreatedEventArgs> Response;

        internal event EventHandler<RequestEventArgs> Request;

        internal event EventHandler<RequestEventArgs> RequestFinished;

        internal event EventHandler<RequestEventArgs> RequestFailed;

        internal FrameManager FrameManager { get; set; }
        #endregion

        #region Public Methods

        internal async Task InitializeAsync()
        {
            await _client.SendAsync("Network.enable").ConfigureAwait(false);
            if (_ignoreHTTPSErrors)
            {
                await _client.SendAsync("Security.setIgnoreCertificateErrors", new SecuritySetIgnoreCertificateErrorsRequest
                {
                    Ignore = true
                }).ConfigureAwait(false);
            }
        }

        internal Task AuthenticateAsync(Credentials credentials)
        {
            _credentials = credentials;
            return UpdateProtocolRequestInterceptionAsync();
        }

        internal Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
        {
            _extraHTTPHeaders = new Dictionary<string, string>();

            foreach (var item in extraHTTPHeaders)
            {
                _extraHTTPHeaders[item.Key.ToLower(CultureInfo.CurrentCulture)] = item.Value;
            }
            return _client.SendAsync("Network.setExtraHTTPHeaders", new NetworkSetExtraHTTPHeadersRequest
            {
                Headers = _extraHTTPHeaders
            });
        }

        internal async Task SetOfflineModeAsync(bool value)
        {
            _emulatedNetworkConditions.Offline = value;
            await UpdateNetworkConditionsAsync().ConfigureAwait(false);
        }

        internal async Task EmulateNetworkConditionsAsync(NetworkConditions networkConditions)
        {
            _emulatedNetworkConditions.Upload = networkConditions?.Upload ?? -1;
            _emulatedNetworkConditions.Download = networkConditions?.Download ?? -1;
            _emulatedNetworkConditions.Latency = networkConditions?.Latency ?? 0;
            await UpdateNetworkConditionsAsync().ConfigureAwait(false);
        }

        private Task UpdateNetworkConditionsAsync()
            => _client.SendAsync("Network.emulateNetworkConditions", new NetworkEmulateNetworkConditionsRequest
            {
                Offline = _emulatedNetworkConditions.Offline,
                Latency = _emulatedNetworkConditions.Latency,
                UploadThroughput = _emulatedNetworkConditions.Upload,
                DownloadThroughput = _emulatedNetworkConditions.Download,
            });

        internal Task SetUserAgentAsync(string userAgent)
            => _client.SendAsync("Network.setUserAgentOverride", new NetworkSetUserAgentOverrideRequest
            {
                UserAgent = userAgent
            });

        internal Task SetCacheEnabledAsync(bool enabled)
        {
            _userCacheDisabled = !enabled;
            return UpdateProtocolCacheDisabledAsync();
        }

        internal Task SetRequestInterceptionAsync(bool value)
        {
            _userRequestInterceptionEnabled = value;
            return UpdateProtocolRequestInterceptionAsync();
        }

        #endregion

        #region Private Methods

        private Task UpdateProtocolCacheDisabledAsync()
            => _client.SendAsync("Network.setCacheDisabled", new NetworkSetCacheDisabledRequest
            {
                CacheDisabled = _userCacheDisabled
            });

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Fetch.requestPaused":
                        await OnRequestPausedAsync(e.MessageData.ToObject<FetchRequestPausedResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Fetch.authRequired":
                        await OnAuthRequiredAsync(e.MessageData.ToObject<FetchAuthRequiredResponse>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestWillBeSent":
                        await OnRequestWillBeSentAsync(e.MessageData.ToObject<RequestWillBeSentPayload>(true)).ConfigureAwait(false);
                        break;
                    case "Network.requestServedFromCache":
                        OnRequestServedFromCache(e.MessageData.ToObject<RequestServedFromCacheResponse>(true));
                        break;
                    case "Network.responseReceived":
                        OnResponseReceived(e.MessageData.ToObject<ResponseReceivedResponse>(true));
                        break;
                    case "Network.loadingFinished":
                        OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedResponse>(true));
                        break;
                    case "Network.loadingFailed":
                        OnLoadingFailed(e.MessageData.ToObject<LoadingFailedResponse>(true));
                        break;
                }
            }
            catch (Exception ex)
            {
                var message = $"NetworkManager failed to process {e.MessageID}. {ex.Message}. {ex.StackTrace}";
                _logger.LogError(ex, message);
                _client.Close(message);
            }
        }

        private void OnLoadingFailed(LoadingFailedResponse e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            if (_requestIdToRequest.TryGetValue(e.RequestId, out var request))
            {
                request.Failure = e.ErrorText;
                request.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

                ForgetRequest(request, true);

                RequestFailed?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void OnLoadingFinished(LoadingFinishedResponse e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            if (_requestIdToRequest.TryGetValue(e.RequestId, out var request))
            {
                request.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

                ForgetRequest(request, true);

                RequestFinished?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void ForgetRequest(Request request, bool events)
        {
            _requestIdToRequest.TryRemove(request.RequestId, out _);

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            if (events)
            {
                _requestIdToRequestWillBeSentEvent.TryRemove(request.RequestId, out _);
                _requestIdToRequestPausedEvent.TryRemove(request.RequestId, out _);
            }
        }

        private void OnResponseReceived(ResponseReceivedResponse e)
        {
            // FileUpload sends a response without a matching request.
            if (_requestIdToRequest.TryGetValue(e.RequestId, out var request))
            {
                var response = new Response(
                    _client,
                    request,
                    e.Response);

                request.Response = response;

                Response?.Invoke(this, new ResponseCreatedEventArgs
                {
                    Response = response
                });
            }
        }

        private async Task OnAuthRequiredAsync(FetchAuthRequiredResponse e)
        {
            var response = "Default";
            if (_attemptedAuthentications.Contains(e.RequestId))
            {
                response = "CancelAuth";
            }
            else if (_credentials != null)
            {
                response = "ProvideCredentials";
                _attemptedAuthentications.Add(e.RequestId);
            }
            var credentials = _credentials ?? new Credentials();
            try
            {
                await _client.SendAsync("Fetch.continueWithAuth", new ContinueWithAuthRequest
                {
                    RequestId = e.RequestId,
                    AuthChallengeResponse = new ContinueWithAuthRequestChallengeResponse
                    {
                        Response = response,
                        Username = credentials.Username,
                        Password = credentials.Password
                    }
                }).ConfigureAwait(false);
            }
            catch (PuppeteerException ex)
            {
                _logger.LogError(ex.ToString());
            }
        }

        private async Task OnRequestPausedAsync(FetchRequestPausedResponse e)
        {
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                try
                {
                    await _client.SendAsync("Fetch.continueRequest", new FetchContinueRequestRequest
                    {
                        RequestId = e.RequestId
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            var requestId = e.NetworkId;
            var interceptionId = e.RequestId;

            if (string.IsNullOrEmpty(requestId))
            {
                return;
            }

            var hasRequestWillBeSentEvent = _requestIdToRequestWillBeSentEvent.TryGetValue(requestId, out var requestWillBeSentEvent);

            // redirect requests have the same `requestId`

            if (hasRequestWillBeSentEvent &&
                (requestWillBeSentEvent.Request.Url != e.Request.Url ||
                 requestWillBeSentEvent.Request.Method != e.Request.Method))
            {
                _requestIdToRequestWillBeSentEvent.TryRemove(requestId, out _);
                hasRequestWillBeSentEvent = false;
            }

            if (hasRequestWillBeSentEvent)
            {
                await OnRequestAsync(requestWillBeSentEvent, interceptionId).ConfigureAwait(false);
                _requestIdToRequestWillBeSentEvent.TryRemove(requestId, out _);
            }
            else
            {
                _requestIdToRequestPausedEvent[requestId] = e;
            }
        }

        private async Task OnRequestAsync(RequestWillBeSentPayload e, string interceptionId)
        {
            Request request;
            var redirectChain = new List<Request>();
            if (e.RedirectResponse != null)
            {
                _requestIdToRequest.TryGetValue(e.RequestId, out request);
                // If we connect late to the target, we could have missed the requestWillBeSent event.
                if (request != null)
                {
                    HandleRequestRedirect(request, e.RedirectResponse);
                    redirectChain = request.RedirectChainList;
                }
            }
            if (!_requestIdToRequest.TryGetValue(e.RequestId, out var currentRequest) ||
              currentRequest.Frame == null)
            {
                var frame = !string.IsNullOrEmpty(e.FrameId) ? await FrameManager.TryGetFrameAsync(e.FrameId).ConfigureAwait(false) : null;

                request = new Request(
                    _client,
                    frame,
                    interceptionId,
                    _userRequestInterceptionEnabled,
                    e,
                    redirectChain);

                _requestIdToRequest[e.RequestId] = request;

                Request?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            if (_requestIdToRequest.TryGetValue(response.RequestId, out var request))
            {
                request.FromMemoryCache = true;
            }
        }

        private void HandleRequestRedirect(Request request, ResponsePayload responseMessage)
        {
            var response = new Response(
                _client,
                request,
                responseMessage);

            request.Response = response;
            request.RedirectChainList.Add(request);
            response.BodyLoadedTaskWrapper.TrySetException(
                new PuppeteerException("Response body is unavailable for redirect responses"));

            ForgetRequest(request, false);

            Response?.Invoke(this, new ResponseCreatedEventArgs
            {
                Response = response
            });

            RequestFinished?.Invoke(this, new RequestEventArgs
            {
                Request = request
            });
        }

        private async Task OnRequestWillBeSentAsync(RequestWillBeSentPayload e)
        {
            // Request interception doesn't happen for data URLs with Network Service.
            if (_userRequestInterceptionEnabled && !e.Request.Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                var hasRequestPausedEvent = _requestIdToRequestPausedEvent.TryGetValue(e.RequestId, out var requestPausedEvent);
                _requestIdToRequestWillBeSentEvent[e.RequestId] = e;

                if (hasRequestPausedEvent)
                {
                    var interceptionId = requestPausedEvent.RequestId;
                    await OnRequestAsync(e, interceptionId).ConfigureAwait(false);
                    _requestIdToRequestPausedEvent.TryRemove(e.RequestId, out _);
                }

                return;
            }
            await OnRequestAsync(e, null).ConfigureAwait(false);
        }

        private async Task UpdateProtocolRequestInterceptionAsync()
        {
            var enabled = _userRequestInterceptionEnabled || _credentials != null;

            if (enabled == _protocolRequestInterceptionEnabled)
            {
                return;
            }
            _protocolRequestInterceptionEnabled = enabled;
            if (enabled)
            {
                await Task.WhenAll(
                    UpdateProtocolCacheDisabledAsync(),
                    _client.SendAsync("Fetch.enable", new FetchEnableRequest
                    {
                        HandleAuthRequests = true,
                        Patterns = new[] { new FetchEnableRequest.Pattern { UrlPattern = "*" } }
                    })).ConfigureAwait(false);
            }
            else
            {
                await Task.WhenAll(
                    UpdateProtocolCacheDisabledAsync(),
                    _client.SendAsync("Fetch.disable")).ConfigureAwait(false);
            }
        }

        #endregion
    }
}
