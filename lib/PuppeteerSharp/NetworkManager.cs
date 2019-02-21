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
        private readonly ConcurrentDictionary<string, NetworkRequest> _requests = new ConcurrentDictionary<string, NetworkRequest>();
        private readonly MultiMap<string, string> _requestHashToRequestIds = new MultiMap<string, string>();
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
            if (_requests.TryRemove(e.RequestId, out var request))
            {
                request.RawRequest.Failure = e.ErrorText;
                request.RawRequest.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

                if (request.InterceptionId != null)
                {
                    _attemptedAuthentications.Remove(request.InterceptionId);
                }

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
            if (_requests.TryRemove(e.RequestId, out var request))
            {
                request.RawRequest.Response?.BodyLoadedTaskWrapper.TrySetResult(true);

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
            if (_requests.TryGetValue(e.RequestId, out var request))
            {
                var response = new Response(
                    _client,
                    request,
                    e.Response);

                request.RawRequest.Response = response;

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
            if (requestId != null && _requests.TryGetValue(requestId, out var request))
            {
                if (request.RequestWillBeSentEvent != null)
                {
                    await OnRequestAsync(request, e.InterceptionId);
                    _requestHashToRequestIds.Delete(requestHash, requestId);
                }
            }
        }

        private async Task OnRequestAsync(RequestWillBeSentPayload e, string interceptionId)
        {
            var redirectChain = new List<Request>();
            if (e.RedirectResponse != null)
            {
                // If we connect late to the target, we could have missed the requestWillBeSent event.
                if (_requests.TryGetValue(e.RequestId, out var redirectRequest))
                {
                    HandleRequestRedirect(redirectRequest, e.RedirectResponse);
                    redirectChain = redirectRequest.RawRequest.RedirectChainList;
                }
            }
            if (!_requests.TryGetValue(e.RequestId, out var request) ||
              request.RawRequest.Frame == null)
            {
                var frame = await FrameManager?.GetFrameAsync(e.FrameId);

                request.RawRequest = new Request(
                    _client,
                    frame,
                    interceptionId,
                    _userRequestInterceptionEnabled,
                    e,
                    redirectChain);

                _requests[e.RequestId] = request;

                Request?.Invoke(this, new RequestEventArgs
                {
                    Request = request
                });
            }
        }

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            if (_requests.TryGetValue(response.RequestId, out var request))
            {
                request.RawRequest.FromMemoryCache = true;
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
                _requests.TryRemove(request.RequestId, out _);
            }

            if (request.InterceptionId != null)
            {
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

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
            if (_protocolRequestInterceptionEnabled && !e.Request.Url.StartsWith("data:", StringComparison.InvariantCultureIgnoreCase)
                && _requests.TryGetValue(e.RequestId, out var request))
            {
                if (request.InterceptionId != null)
                {
                    await OnRequestAsync(e, request.InterceptionId);
                }
                else
                {
                    _requestHashToRequestIds.Add(e.Request.Hash, e.RequestId);
                    request.RequestWillBeSentEvent = e;
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

        internal sealed class NetworkRequest
        {
            internal RequestWillBeSentPayload RequestWillBeSentEvent { get; set; }
            internal Request RawRequest { get; set; }
            internal string InterceptionId
            {
                get => RawRequest?.InterceptionId;
                set => RawRequest.InterceptionId = value;
            }

            public static implicit operator Request(NetworkRequest request) => request.RawRequest;
            public static implicit operator RequestWillBeSentPayload(NetworkRequest request) => request.RequestWillBeSentEvent;
        }
    }
}