using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Messaging;

namespace PuppeteerSharp
{
    internal class NetworkManager
    {
        #region Private members

        private readonly CDPSession _client;
        private readonly Dictionary<string, Request> _requestIdToRequest = new Dictionary<string, Request>();
        private readonly Dictionary<string, Request> _interceptionIdToRequest = new Dictionary<string, Request>();
        private readonly MultiMap<string, string> _requestHashToRequestIds = new MultiMap<string, string>();
        private readonly MultiMap<string, string> _requestHashToInterceptionIds = new MultiMap<string, string>();
        private readonly FrameManager _frameManager;
        private readonly ILogger _logger;
        private Dictionary<string, string> _extraHTTPHeaders;
        private bool _offine;
        private Credentials _credentials;
        private List<string> _attemptedAuthentications = new List<string>();
        private bool _userRequestInterceptionEnabled;
        private bool _protocolRequestInterceptionEnabled;

        #endregion

        internal NetworkManager(CDPSession client, FrameManager frameManager)
        {
            _frameManager = frameManager;
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
        #endregion

        #region Public Methods

        internal async Task AuthenticateAsync(Credentials credentials)
        {
            _credentials = credentials;
            await UpdateProtocolRequestInterceptionAsync();
        }

        internal async Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
        {
            _extraHTTPHeaders = new Dictionary<string, string>();

            foreach (var item in extraHTTPHeaders)
            {
                _extraHTTPHeaders[item.Key.ToLower()] = item.Value;
            }
            await _client.SendAsync("Network.setExtraHTTPHeaders", new Dictionary<string, object>
            {
                {"headers", _extraHTTPHeaders}
            });
        }

        internal async Task SetOfflineModeAsync(bool value)
        {
            if (_offine != value)
            {
                _offine = value;

                await _client.SendAsync("Network.emulateNetworkConditions", new Dictionary<string, object>
                {
                    { "offline", value},
                    { "latency", 0},
                    { "downloadThroughput", -1},
                    { "uploadThroughput", -1}
                });
            }
        }

        internal Task SetUserAgentAsync(string userAgent)
            => _client.SendAsync("Network.setUserAgentOverride", new Dictionary<string, object>
            {
                { "userAgent", userAgent }
            });

        internal async Task SetRequestInterceptionAsync(bool value)
        {
            _userRequestInterceptionEnabled = value;
            await UpdateProtocolRequestInterceptionAsync();
        }

        #endregion

        #region Private Methods

        private async void Client_MessageReceived(object sender, MessageEventArgs e)
        {
            switch (e.MessageID)
            {
                case "Network.requestWillBeSent":
                    OnRequestWillBeSent(e.MessageData.ToObject<RequestWillBeSentResponse>());
                    break;
                case "Network.requestIntercepted":
                    await OnRequestInterceptedAsync(e.MessageData.ToObject<RequestInterceptedResponse>());
                    break;
                case "Network.requestServedFromCache":
                    OnRequestServedFromCache(e.MessageData.ToObject<RequestServedFromCacheResponse>());
                    break;
                case "Network.responseReceived":
                    OnResponseReceived(e.MessageData.ToObject<ResponseReceivedResponse>());
                    break;
                case "Network.loadingFinished":
                    OnLoadingFinished(e.MessageData.ToObject<LoadingFinishedResponse>());
                    break;
                case "Network.loadingFailed":
                    OnLoadingFailed(e.MessageData.ToObject<LoadingFailedResponse>());
                    break;
            }
        }

        private void OnLoadingFailed(LoadingFailedResponse e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            if (_requestIdToRequest.TryGetValue(e.RequestId, out var request))
            {
                request.Failure = e.ErrorText;
                request.Response?.BodyLoadedTaskWrapper.SetResult(true);
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _interceptionIdToRequest.Remove(request.InterceptionId);
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
                request.Response.BodyLoadedTaskWrapper.SetResult(true);
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _interceptionIdToRequest.Remove(request.InterceptionId);
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
                    e.Response.Status,
                    e.Response.Headers,
                    e.Response.FromDiskCache,
                    e.Response.FromServiceWorker,
                    e.Response.SecurityDetails);

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
                    await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                    {
                        {"interceptionId", e.InterceptionId},
                        {"authChallengeResponse", new
                            {
                                response,
                                username = credentials.Username,
                                password = credentials.Password
                            }
                        }
                    });
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
                    await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                    {
                        { "interceptionId", e.InterceptionId}
                    });
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (!string.IsNullOrEmpty(e.RedirectUrl))
            {
                var request = _interceptionIdToRequest[e.InterceptionId];

                HandleRequestRedirect(request, e.ResponseStatusCode, e.ResponseHeaders, false, false);
                HandleRequestStart(
                    request.RequestId,
                    e.InterceptionId,
                    e.RedirectUrl,
                    e.IsNavigationRequest,
                    e.ResourceType,
                    e.Request,
                    e.FrameId,
                    request.RedirectChainList);
                return;
            }

            var requestHash = e.Request.Hash;
            var requestId = _requestHashToRequestIds.FirstValue(requestHash);
            if (requestId != null)
            {
                _requestHashToRequestIds.Delete(requestHash, requestId);
                HandleRequestStart(
                    requestId,
                    e.InterceptionId,
                    e.Request.Url,
                    e.IsNavigationRequest,
                    e.ResourceType,
                    e.Request,
                    e.FrameId,
                    new List<Request>());
            }
            else
            {
                _requestHashToInterceptionIds.Add(requestHash, e.InterceptionId);
                HandleRequestStart(
                    null,
                    e.InterceptionId,
                    e.Request.Url,
                    e.IsNavigationRequest,
                    e.ResourceType,
                    e.Request,
                    e.FrameId,
                    new List<Request>());
            }
        }

        private void OnRequestServedFromCache(RequestServedFromCacheResponse response)
        {
            if (_requestIdToRequest.TryGetValue(response.RequestId, out var request))
            {
                request.FromMemoryCache = true;
            }
        }

        private void HandleRequestStart(
            string requestId,
            string interceptionId,
            string url,
            bool isNavigationRequest,
            ResourceType resourceType,
            Payload requestPayload,
            string frameId,
            List<Request> redirectChain)
        {
            Frame frame = null;

            if (!string.IsNullOrEmpty(frameId))
            {
                _frameManager.Frames.TryGetValue(frameId, out frame);
            }

            var request = new Request(
                _client,
                requestId,
                interceptionId,
                isNavigationRequest,
                _userRequestInterceptionEnabled,
                url,
                resourceType,
                requestPayload,
                frame,
                redirectChain);

            if (!string.IsNullOrEmpty(requestId))
            {
                _requestIdToRequest.Add(requestId, request);
            }
            if (!string.IsNullOrEmpty(interceptionId))
            {
                _interceptionIdToRequest.Add(interceptionId, request);
            }

            Request(this, new RequestEventArgs
            {
                Request = request
            });
        }

        private void HandleRequestRedirect(
            Request request,
            HttpStatusCode redirectStatus,
            Dictionary<string, object> redirectHeaders,
            bool fromDiskCache,
            bool fromServiceWorker,
            SecurityDetails securityDetails = null)
        {
            var response = new Response(
                _client,
                request,
                redirectStatus,
                redirectHeaders,
                fromDiskCache,
                fromServiceWorker,
                securityDetails);

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
                _interceptionIdToRequest.Remove(request.InterceptionId);
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

        private void OnRequestWillBeSent(RequestWillBeSentResponse e)
        {
            var redirectChain = new List<Request>();

            if (_protocolRequestInterceptionEnabled)
            {
                // All redirects are handled in requestIntercepted.
                if (e.RedirectResponse != null)
                {
                    return;
                }
                var requestHash = e.Request.Hash;
                var interceptionId = _requestHashToInterceptionIds.FirstValue(requestHash);
                if (interceptionId != null && _interceptionIdToRequest.TryGetValue(interceptionId, out var request))
                {
                    request.RequestId = e.RequestId;
                    _requestIdToRequest[e.RequestId] = request;
                    _requestHashToInterceptionIds.Delete(requestHash, interceptionId);
                }
                else
                {
                    _requestHashToRequestIds.Add(requestHash, e.RequestId);
                }
                return;
            }

            if (e.RedirectResponse != null && _requestIdToRequest.ContainsKey(e.RequestId))
            {
                var request = _requestIdToRequest[e.RequestId];
                // If we connect late to the target, we could have missed the requestWillBeSent event.
                HandleRequestRedirect(
                    request,
                    e.RedirectResponse.Status,
                    e.RedirectResponse.Headers,
                    e.RedirectResponse.FromDiskCache,
                    e.RedirectResponse.FromServiceWorker,
                    e.RedirectResponse.SecurityDetails);

                redirectChain = request.RedirectChainList;
            }
            var isNavigationRequest = e.RequestId == e.LoaderId && e.Type == ResourceType.Document;

            HandleRequestStart(e.RequestId, null, e.Request.Url, isNavigationRequest, e.Type, e.Request, e.FrameId, redirectChain);
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
                _client.SendAsync("Network.setCacheDisabled", new Dictionary<string, object>
                {
                    { "cacheDisabled", enabled}
                }),
                _client.SendAsync("Network.setRequestInterception", new Dictionary<string, object>
                {
                    { "patterns", patterns}
                })
            );
        }
        #endregion
    }
}