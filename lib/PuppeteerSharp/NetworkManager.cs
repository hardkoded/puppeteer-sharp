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
        private readonly CDPSession _client;
        private readonly ILogger _logger;
        private readonly ConcurrentSet<string> _attemptedAuthentications = new();
        private readonly bool _ignoreHTTPSErrors;
        private readonly InternalNetworkConditions _emulatedNetworkConditions = new()
        {
            Offline = false,
            Upload = -1,
            Download = -1,
            Latency = 0,
        };

        private readonly NetworkEventManager _networkEventManager = new();
        private readonly bool _protocolRequestInterceptionEnabled;

        private Dictionary<string, string> _extraHTTPHeaders;
        private Credentials _credentials;
        private bool _userRequestInterceptionEnabled;
        private bool _userCacheDisabled;

        internal NetworkManager(CDPSession client, bool ignoreHTTPSErrors, FrameManager frameManager)
        {
            FrameManager = frameManager;
            _client = client;
            _ignoreHTTPSErrors = ignoreHTTPSErrors;
            _client.MessageReceived += Client_MessageReceived;
            _logger = _client.Connection.LoggerFactory.CreateLogger<NetworkManager>();
        }

        internal Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();

        internal event EventHandler<ResponseCreatedEventArgs> Response;

        internal event EventHandler<RequestEventArgs> Request;

        internal event EventHandler<RequestEventArgs> RequestFinished;

        internal event EventHandler<RequestEventArgs> RequestFailed;

        internal event EventHandler<RequestEventArgs> RequestServedFromCache;

        internal FrameManager FrameManager { get; set; }

        internal int NumRequestsInProgress => _networkEventManager.NumRequestsInProgress;

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
                        OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedEventResponse>(true));
                        break;
                    case "Network.loadingFailed":
                        OnLoadingFailed(e.MessageData.ToObject<LoadingFailedResponse>(true));
                        break;
                    case "Network.responseReceivedExtraInfo":
                        OnResponseReceivedExtraInfo(e.MessageData.ToObject<ResponseReceivedExtraInfoResponse>(true));
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

        private void OnResponseReceivedExtraInfo(ResponseReceivedExtraInfoResponse responseReceivedExtraInfoResponse)
        {
            var redirectInfo = _networkEventManager.TakeQueuedRedirectInfo(
                responseReceivedExtraInfoResponse.RequestId);

            if (redirectInfo != null) {
                _networkEventManager.ResponseExtraInfo(responseReceivedExtraInfoResponse.RequestId).Add(responseReceivedExtraInfoResponse);
                OnRequest(redirectInfo.Event, redirectInfo.FetchRequestId);
                return;
            }

            // We may have skipped response and loading events because we didn't have
            // this ExtraInfo event yet. If so, emit those events now.
            var queuedEvents = _networkEventManager.GetQueuedEventGroup(
              responseReceivedExtraInfoResponse.RequestId);

            if (queuedEvents != null) {
                OnResponseReceived(queuedEvents.ResponseReceivedEvent, responseReceivedExtraInfoResponse);

                if (queuedEvents.LoadingFinishedEvent != null) {
                    OnLoadingFinished(queuedEvents.LoadingFinishedEvent);
                }

                if (queuedEvents.LoadingFailedEvent != null) {
                    OnLoadingFailed(queuedEvents.LoadingFailedEvent);
                }
                return;
            }

            // Wait until we get another event that can use this ExtraInfo event.
            _networkEventManager.ResponseExtraInfo(responseReceivedExtraInfoResponse.RequestId).Add(responseReceivedExtraInfoResponse);
        }

        private void OnLoadingFailed(LoadingFailedEventResponse e)
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

        private void OnLoadingFinished(LoadingFinishedEventResponse e)
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
            _networkEventManager.ForgetRequest(request.RequestId);

            _requestIdToRequest.TryRemove(request.RequestId, out _);

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            if (events)
            {
                _networkEventManager.Forget(request.RequestId);
            }
        }

        private void OnResponseReceived(ResponseReceivedResponse e, ResponseReceivedExtraInfoResponse extraInfo)
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

            var requestWillBeSentEvent =
              _networkEventManager.GetRequestWillBeSent(e.NetworkRequestId);

                    // redirect requests have the same `requestId`,
                    if (
                        requestWillBeSentEvent &&
                        (requestWillBeSentEvent.request.url !== event.request.url ||
                    requestWillBeSentEvent.request.method !== event.request.method)
                ) {
                    this._networkEventManager.forgetRequestWillBeSent(networkRequestId);
                    return;
                }
                return requestWillBeSentEvent;
       

            if (requestWillBeSentEvent) {
                this._onRequest(requestWillBeSentEvent, fetchRequestId);
            } else {
                this._networkEventManager.storeRequestPaused(networkRequestId, event);
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

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            if (_requestIdToRequest.TryGetValue(response.RequestId, out var request))
            {
                request.FromMemoryCache = true;
            }
            RequestServedFromCache?.Invoke(this, new RequestEventArgs { Request = request });
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
    }
}
