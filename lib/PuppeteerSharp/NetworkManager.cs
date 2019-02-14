using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly IDictionary<string, Request> _requestIdToRequest = new ConcurrentDictionary<string, Request>();
        private readonly IDictionary<string, RequestWillBeSentPayload> _requestIdToRequestWillBeSentEvent =
            new ConcurrentDictionary<string, RequestWillBeSentPayload>();
        private readonly MultiMap<string, string> _requestHashToRequestIds = new MultiMap<string, string>();
        private readonly MultiMap<string, string> _requestHashToInterceptionIds = new MultiMap<string, string>();
        private readonly ILogger _logger;
        private Dictionary<string, string> _extraHTTPHeaders;
        private bool _offine;
        private Credentials _credentials;
        private List<string> _attemptedAuthentications = new List<string>();
        private bool _userRequestInterceptionEnabled;
        private bool _protocolRequestInterceptionEnabled;

        #endregion

        internal NetworkManager(CDPSession client)
        {
            FrameManager = null;
            _client = client;
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
                _extraHTTPHeaders[item.Key.ToLower()] = item.Value;
            }
            return _client.SendAsync("Network.setExtraHTTPHeaders", new NetworkSetExtraHTTPHeadersRequest
            {
                Headers = _extraHTTPHeaders
            });
        }

        internal async Task SetOfflineModeAsync(bool value)
        {
            if (_offine != value)
            {
                _offine = value;

                await _client.SendAsync("Network.emulateNetworkConditions", new NetworkEmulateNetworkConditionsRequest
                {
                    Offline = value,
                    Latency = 0,
                    DownloadThroughput = -1,
                    UploadThroughput = -1
                }).ConfigureAwait(false);
            }
        }

        internal Task SetUserAgentAsync(string userAgent)
            => _client.SendAsync("Network.setUserAgentOverride", new NetworkSetUserAgentOverrideRequest
            {
                UserAgent = userAgent
            });

        internal Task SetRequestInterceptionAsync(bool value)
        {
            _userRequestInterceptionEnabled = value;
            return UpdateProtocolRequestInterceptionAsync();
        }

        #endregion

        #region Private Methods

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            try
            {
                switch (e.MessageID)
                {
                    case "Network.requestWillBeSent":
                        await OnRequestWillBeSentAsync(e.MessageData.ToObject<RequestWillBeSentPayload>(true));
                        break;
                    case "Network.requestIntercepted":
                        await OnRequestInterceptedAsync(e.MessageData.ToObject<RequestInterceptedResponse>(true)).ConfigureAwait(false);
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
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _attemptedAuthentications.Remove(request.InterceptionId);
                }

                RequestFailed(this, new RequestEventArgs
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
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _attemptedAuthentications.Remove(request.InterceptionId);
                }

                RequestFinished?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
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

        private async Task OnRequestInterceptedAsync(RequestInterceptedResponse e)
        {
            if (e.AuthChallenge != null)
            {
                var response = "Default";
                if (_attemptedAuthentications.Contains(e.InterceptionId))
                {
                    response = "CancelAuth";
                }
                else if (_credentials != null)
                {
                    response = "ProvideCredentials";
                    _attemptedAuthentications.Add(e.InterceptionId);
                }
                var credentials = _credentials ?? new Credentials();
                try
                {
                    await _client.SendAsync("Network.continueInterceptedRequest", new NetworkContinueInterceptedRequestRequest
                    {
                        InterceptionId = e.InterceptionId,
                        AuthChallengeResponse = new NetworkContinueInterceptedRequestChallengeResponse
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
                return;
            }
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                try
                {
                    await _client.SendAsync("Network.continueInterceptedRequest", new NetworkContinueInterceptedRequestRequest
                    {
                        InterceptionId = e.InterceptionId
                    }).ConfigureAwait(false);
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            var requestHash = e.Request.Hash;
            var requestId = _requestHashToRequestIds.FirstValue(requestHash);
            if (requestId != null)
            {
                _requestIdToRequestWillBeSentEvent.TryGetValue(requestId, out var requestWillBeSentEvent);

                if (requestWillBeSentEvent != null)
                {
                    await OnRequestAsync(requestWillBeSentEvent, e.InterceptionId);
                    _requestHashToRequestIds.Delete(requestHash, requestId);
                    _requestIdToRequestWillBeSentEvent.Remove(requestId);
                }
            }
            else
            {
                _requestHashToInterceptionIds.Add(requestHash, e.InterceptionId);
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
                var frame = await FrameManager?.GetFrameAsync(e.FrameId);

                request = new Request(
                    _client,
                    frame,
                    interceptionId,
                    _userRequestInterceptionEnabled,
                    e,
                    redirectChain);

                _requestIdToRequest[e.RequestId] = request;

                Request(this, new RequestEventArgs
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

            if (request.RequestId != null)
            {
                _requestIdToRequest.Remove(request.RequestId);
            }

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            Response(this, new ResponseCreatedEventArgs
            {
                Response = response
            });

            RequestFinished(this, new RequestEventArgs
            {
                Request = request
            });
        }

        private async Task OnRequestWillBeSentAsync(RequestWillBeSentPayload e)
        {
            // Request interception doesn't happen for data URLs with Network Service.
            if (_protocolRequestInterceptionEnabled && !e.Request.Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase))
            {
                var requestHash = e.Request.Hash;
                var interceptionId = _requestHashToInterceptionIds.FirstValue(requestHash);
                if (interceptionId != null)
                {
                    await OnRequestAsync(e, interceptionId);
                    _requestHashToInterceptionIds.Delete(requestHash, interceptionId);
                }
                else
                {
                    _requestHashToRequestIds.Add(requestHash, e.RequestId);
                    _requestIdToRequestWillBeSentEvent.Add(e.RequestId, e);
                }
                return;
            }
            await OnRequestAsync(e, null);
        }

        private async Task UpdateProtocolRequestInterceptionAsync()
        {
            var enabled = _userRequestInterceptionEnabled || _credentials != null;

            if (enabled == _protocolRequestInterceptionEnabled)
            {
                return;
            }

            _protocolRequestInterceptionEnabled = enabled;
            var patterns = enabled ?
                new object[] { new KeyValuePair<string, string>("urlPattern", "*") } :
                Array.Empty<object>();

            await Task.WhenAll(
                _client.SendAsync("Network.setCacheDisabled", new NetworkSetCacheDisabledRequest
                {
                    CacheDisabled = enabled
                }),
                _client.SendAsync("Network.setRequestInterception", new NetworkSetRequestInterceptionRequest
                {
                    Patterns = patterns
                })
            ).ConfigureAwait(false);
        }
        #endregion
    }
}