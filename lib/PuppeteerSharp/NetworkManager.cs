using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    internal class NetworkManager
    {
        #region Private members

        private readonly CDPSession _client;
        private Dictionary<string, Request> _requestIdToRequest = new Dictionary<string, Request>();
        private Dictionary<string, Request> _interceptionIdToRequest = new Dictionary<string, Request>();
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

        internal async Task SetUserAgentAsync(string userAgent)
        {
            await _client.SendAsync("Network.setUserAgentOverride", new Dictionary<string, object>
            {
                { "userAgent", userAgent }
            });
        }

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
                    OnRequestWillBeSent(e);
                    break;
                case "Network.requestIntercepted":
                    await OnRequestInterceptedAsync(e);
                    break;
                case "Network.responseReceived":
                    OnResponseReceived(e);
                    break;
                case "Network.loadingFinished":
                    OnLoadingFinished(e);
                    break;
                case "Network.loadingFailed":
                    OnLoadingFailed(e);
                    break;
            }
        }

        private void OnLoadingFailed(MessageEventArgs e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            string requestId = e.MessageData.requestId.ToString();
            if (_requestIdToRequest.TryGetValue(requestId, out var request))
            {
                request.Failure = e.MessageData.errorText.ToString();
                request.CompleteTaskWrapper.SetResult(true);
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

        private void OnLoadingFinished(MessageEventArgs e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            string requestId = e.MessageData.requestId.ToString();
            if (_requestIdToRequest.TryGetValue(requestId, out var request))
            {
                request.CompleteTaskWrapper.SetResult(true);
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

        private void OnResponseReceived(MessageEventArgs e)
        {
            // FileUpload sends a response without a matching request.
            string requestId = e.MessageData.requestId.ToString();
            if (_requestIdToRequest.TryGetValue(requestId, out var request))
            {
                var response = new Response(
                    _client,
                    request,
                    (HttpStatusCode)e.MessageData.response.status,
                    ((JObject)e.MessageData.response.headers).ToObject<Dictionary<string, object>>(),
                    e.MessageData.response.securityDetails?.ToObject<SecurityDetails>());

                request.Response = response;

                Response?.Invoke(this, new ResponseCreatedEventArgs
                {
                    Response = response
                });
            }
        }

        private async Task OnRequestInterceptedAsync(MessageEventArgs e)
        {
            if (e.MessageData.authChallenge != null)
            {
                var response = "Default";
                if (_attemptedAuthentications.Contains(e.MessageData.interceptionId.ToString()))
                {
                    response = "CancelAuth";
                }
                else if (_credentials != null)
                {
                    response = "ProvideCredentials";
                    _attemptedAuthentications.Add(e.MessageData.interceptionId.ToString());
                }
                var credentials = _credentials ?? new Credentials();
                try
                {
                    await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                    {
                        {"interceptionId", e.MessageData.interceptionId.ToString()},
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
                        { "interceptionId", e.MessageData.interceptionId.ToString()}
                    });
                }
                catch (PuppeteerException ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }

            if (!string.IsNullOrEmpty(e.MessageData.redirectUrl?.ToString()))
            {
                var request = _interceptionIdToRequest[e.MessageData.interceptionId.ToString()];

                HandleRequestRedirect(request, (HttpStatusCode)e.MessageData.responseStatusCode, e.MessageData.responseHeaders.ToObject<Dictionary<string, object>>());
                HandleRequestStart(request.RequestId, e.MessageData, e.MessageData.redirectUrl.ToString());
                return;
            }

            string requestHash = e.MessageData.request.ToObject<Payload>().Hash;
            var requestId = _requestHashToRequestIds.FirstValue(requestHash);
            if (requestId != null)
            {
                _requestHashToRequestIds.Delete(requestHash, requestId);
                HandleRequestStart(requestId, e.MessageData);
            }
            else
            {
                _requestHashToInterceptionIds.Add(requestHash, e.MessageData.interceptionId.ToString());
                HandleRequestStart(null, e.MessageData);
            }
        }

        private void HandleRequestStart(string requestId, dynamic messageData, string url = null)
        {
            HandleRequestStart(
                requestId,
                messageData.interceptionId?.ToString(),
                url ?? messageData.request.url?.ToString(),
                (messageData.resourceType ?? messageData.type)?.ToObject<ResourceType>(),
                ((JObject)messageData.request).ToObject<Payload>(),
                messageData.frameId?.ToString());
        }

        private void HandleRequestStart(string requestId, string interceptionId, string url, ResourceType resourceType, Payload requestPayload, string frameId)
        {
            Frame frame = null;

            if (!string.IsNullOrEmpty(frameId))
            {
                _frameManager.Frames.TryGetValue(frameId, out frame);
            }

            var request = new Request(_client, requestId, interceptionId, _userRequestInterceptionEnabled, url,
                                      resourceType, requestPayload, frame);

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

        private void HandleRequestRedirect(Request request, HttpStatusCode redirectStatus, Dictionary<string, object> redirectHeaders, SecurityDetails securityDetails = null)
        {
            var response = new Response(_client, request, redirectStatus, redirectHeaders, securityDetails);
            request.Response = response;
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

        private void OnRequestWillBeSent(MessageEventArgs e)
        {
            if (_protocolRequestInterceptionEnabled)
            {
                // All redirects are handled in requestIntercepted.
                if (e.MessageData.redirectResponse != null)
                {
                    return;
                }
                string requestHash = e.MessageData.request.ToObject<Payload>().Hash;
                var interceptionId = _requestHashToInterceptionIds.FirstValue(requestHash);
                if (interceptionId != null && _interceptionIdToRequest.TryGetValue(interceptionId, out var request))
                {
                    request.RequestId = e.MessageData.requestId.ToString();
                    _requestIdToRequest[e.MessageData.requestId.ToString()] = request;
                    _requestHashToInterceptionIds.Delete(requestHash, interceptionId);
                }
                else
                {
                    _requestHashToRequestIds.Add(requestHash, e.MessageData.requestId.ToString());
                }
                return;
            }

            if (e.MessageData.redirectResponse != null && _requestIdToRequest.ContainsKey(e.MessageData.requestId.ToString()))
            {
                var request = _requestIdToRequest[e.MessageData.requestId.ToString()];
                // If we connect late to the target, we could have missed the requestWillBeSent event.
                HandleRequestRedirect(
                    request,
                    (HttpStatusCode)e.MessageData.redirectResponse.status,
                    e.MessageData.redirectResponse.headers.ToObject<Dictionary<string, object>>(),
                    e.MessageData.redirectResponse.securityDetails?.ToObject<SecurityDetails>());
            }

            HandleRequestStart(e.MessageData.requestId?.ToString(), e.MessageData);
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
