using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp
{
    public class NetworkManager
    {

        #region Private members
        private Session _client;
        private Dictionary<string, Request> _requestIdToRequest = new Dictionary<string, Request>();
        private Dictionary<string, Request> _interceptionIdToRequest = new Dictionary<string, Request>();
        private Dictionary<string, string> _extraHTTPHeaders;
        private bool _offine;
        private Credentials _credentials;
        private List<string> _attemptedAuthentications = new List<string>();
        private bool _userRequestInterceptionEnabled;
        private bool _protocolRequestInterceptionEnabled;

        private List<KeyValuePair<string, string>> _requestHashToRequestIds = new List<KeyValuePair<string, string>>();
        private List<KeyValuePair<string, string>> _requestHashToInterceptionIds = new List<KeyValuePair<string, string>>();
        #endregion

        public NetworkManager(Session client)
        {
            _client = client;
            _client.MessageReceived += Client_MessageReceived;
        }

        #region Public Properties
        public Dictionary<string, string> ExtraHTTPHeaders => _extraHTTPHeaders?.Clone();

        public event EventHandler<ResponseCreatedArgs> ResponseCreated;
        public event EventHandler<RequestEventArgs> RequestCreated;
        public event EventHandler<RequestEventArgs> RequestFinished;
        public event EventHandler<RequestEventArgs> RequestFailed;
        public event EventHandler<ResponseReceivedArgs> ResponseReceivedFinished;

        #endregion


        #region Public Methods

        public async Task AuthenticateAsync(Credentials credentials)
        {
            _credentials = credentials;
            await UpdateProtocolRequestInterceptionAsync();
        }

        public async Task SetExtraHTTPHeadersAsync(Dictionary<string, string> extraHTTPHeaders)
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

        public async Task SetOfflineModeAsync(bool value)
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


        public async Task SetUserAgentAsync(string userAgent)
        {
            await _client.SendAsync("Network.setUserAgentOverride", new Dictionary<string, object>
            {
                { "userAgent", userAgent }
            });
        }

        public async Task SetRequestInterceptionAsync(bool value)
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
            if (_requestIdToRequest.ContainsKey(e.MessageData.requestId.ToString()))
            {
                var request = _requestIdToRequest[e.MessageData.requestId.ToString()];

                request.Failure = e.MessageData.errorText.ToString();
                request.CompleteTaskWrapper.SetResult(true);
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _interceptionIdToRequest.Remove(request.InterceptionId);
                    _attemptedAuthentications.Remove(request.InterceptionId);
                }
                RequestFailed(this, new RequestEventArgs()
                {
                    Request = request
                });
            }
        }

        private void OnLoadingFinished(MessageEventArgs e)
        {
            // For certain requestIds we never receive requestWillBeSent event.
            // @see https://crbug.com/750469
            if (_requestIdToRequest.ContainsKey(e.MessageData.requestId.ToString()))
            {
                var request = _requestIdToRequest[e.MessageData.requestId.ToString()];

                request.CompleteTaskWrapper.SetResult(true);
                _requestIdToRequest.Remove(request.RequestId);

                if (request.InterceptionId != null)
                {
                    _interceptionIdToRequest.Remove(request.InterceptionId);
                    _attemptedAuthentications.Remove(request.InterceptionId);
                }

                RequestFinished?.Invoke(this, new RequestEventArgs()
                {
                    Request = request
                });
            }
        }

        private void OnResponseReceived(MessageEventArgs e)
        {
            // FileUpload sends a response without a matching request.
            if (_requestIdToRequest.ContainsKey(e.MessageData.requestId.ToString()))
            {
                var request = _requestIdToRequest[e.MessageData.requestId.ToString()];
                var response = new Response(
                    _client,
                    request,
                    (HttpStatusCode)e.MessageData.response.status,
                    ((JObject)e.MessageData.response.headers).ToObject<Dictionary<string, object>>(),
                    e.MessageData.response.securityDetails?.ToObject<SecurityDetails>());

                request.Response = response;

                ResponseReceivedFinished?.Invoke(this, new ResponseReceivedArgs()
                {
                    Response = response
                });
            }
        }

        private async Task OnRequestInterceptedAsync(MessageEventArgs e)
        {
            if (e.MessageData.authChallenge)
            {
                var response = "Default";
                if (_attemptedAuthentications.Contains(e.MessageData.interceptionId.ToString()))
                {
                    response = "CancelAuth";
                }
                else if (_credentials != null)
                {
                    response = "ProvideCredentials";
                    _attemptedAuthentications.Add(e.MessageData.interceptionId);
                }
                var credentials = _credentials ?? new Credentials();
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                {
                    {"interceptionId", e.MessageData.interceptionId},
                    {"authChallengeResponse", new { response, credentials.Username, credentials.Password }}
                });
                return;
            }
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object> {
                    { "interceptionId", e.MessageData.interceptionId}
                });
            }

            if (!string.IsNullOrEmpty(e.MessageData.redirectUrl))
            {
                var request = _interceptionIdToRequest[e.MessageData.interceptionId];

                HandleRequestRedirect(request, e.MessageData.responseStatusCode, e.MessageData.responseHeaders);
                HandleRequestStart(
                    request.RequestId,
                    e.MessageData.interceptionId,
                    e.MessageData.redirectUrl,
                    e.MessageData.resourceType,
                    e.MessageData.request);
                return;
            }
            var requestHash = e.MessageData.request.hash;


            if (_requestHashToRequestIds.Any(i => i.Key == requestHash))
            {
                var item = _requestHashToRequestIds.FirstOrDefault(i => i.Key == requestHash);
                var requestId = item.Value;
                _requestHashToRequestIds.Remove(item);
                HandleRequestStart(
                    requestId,
                    e.MessageData.interceptionId,
                    e.MessageData.request.url,
                    e.MessageData.resourceType,
                    e.MessageData.request);
            }
            else
            {
                _requestHashToInterceptionIds.Add(new KeyValuePair<string, string>(requestHash, e.MessageData.interceptionId));
                HandleRequestStart(
                    null,
                    e.MessageData.interceptionId,
                    e.MessageData.request.url,
                    e.MessageData.resourceType,
                    e.MessageData.request);
            }
        }

        private void HandleRequestStart(string requestId, string interceptionId, string url, string resourceType, Payload requestPayload)
        {
            var request = new Request(_client, requestId, interceptionId, _userRequestInterceptionEnabled, url,
                                      resourceType, requestPayload);

            if (!string.IsNullOrEmpty(requestId))
            {
                _requestIdToRequest.Add(requestId, request);
            }
            if (!string.IsNullOrEmpty(interceptionId))
            {
                _interceptionIdToRequest.Add(interceptionId, request);
            }

            RequestCreated(this, new RequestEventArgs()
            {
                Request = request
            });
        }

        private void HandleRequestRedirect(Request request, HttpStatusCode redirectStatus, Dictionary<string, object> redirectHeaders, SecurityDetails securityDetails = null)
        {
            var response = new Response(_client, request, redirectStatus, redirectHeaders, securityDetails);
            request.Response = response;
            _requestIdToRequest.Remove(request.RequestId);

            if (request.InterceptionId != null)
            {
                _interceptionIdToRequest.Remove(request.InterceptionId);
                _attemptedAuthentications.Remove(request.InterceptionId);
            }

            ResponseCreated(this, new ResponseCreatedArgs()
            {
                Response = response
            });

            RequestFinished(this, new RequestEventArgs()
            {
                Request = request
            });
        }

        private void OnRequestWillBeSent(MessageEventArgs e)
        {
            if (_protocolRequestInterceptionEnabled)
            {
                // All redirects are handled in requestIntercepted.
                if (e.MessageData.redirectResponse == null)
                {
                    var requestHash = e.MessageData.request.hash;

                    KeyValuePair<string, string>? interceptionItem = null;

                    if (_requestHashToInterceptionIds.Any(i => i.Key == requestHash))
                    {
                        interceptionItem = _requestHashToInterceptionIds.First(i => i.Key == requestHash);
                    }

                    if (interceptionItem.HasValue && _interceptionIdToRequest.ContainsKey(interceptionItem.Value.Value))
                    {
                        var request = _interceptionIdToRequest[interceptionItem.Value.Value];

                        request.RequestId = e.MessageData.requestId;
                        _requestIdToRequest[e.MessageData.requestId] = request;
                        _requestHashToInterceptionIds.Remove(interceptionItem.Value);
                    }
                    else
                    {
                        _requestHashToRequestIds.Add(new KeyValuePair<string, string>(requestHash, e.MessageData.requestId));
                    }
                    return;
                }
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

            HandleRequestStart(
                e.MessageData.requestId?.ToString(),
                null,
                e.MessageData.request.url?.ToString(),
                e.MessageData.type?.ToString(),
                ((JObject)e.MessageData.request).ToObject<Payload>());
        }


        private async Task UpdateProtocolRequestInterceptionAsync()
        {
            var enabled = _userRequestInterceptionEnabled || _credentials != null;

            if (enabled != _protocolRequestInterceptionEnabled)
            {
                _protocolRequestInterceptionEnabled = enabled;
                var patterns = enabled ?
                    new Dictionary<string, object> { { "urlPattern", "*" } } :
                    new Dictionary<string, object>();

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

        }

        #endregion
    }
}
