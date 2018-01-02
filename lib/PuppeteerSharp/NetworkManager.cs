using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        private Dictionary<string, object> _requestHashToInterceptionIds = new Dictionary<string, object>();
        #endregion

        public NetworkManager(Session client)
        {
            _client = client;
            _client.MessageReceived += Client_MessageReceived;
        }

        #region Public Properties

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

        public Dictionary<string, string> ExtraHTTPHeaders()
        {
            return _extraHTTPHeaders.Clone();
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


        private async void Client_MessageReceived(object sender, PuppeteerSharp.MessageEventArgs e)
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
            throw new NotImplementedException();
        }

        private void OnLoadingFinished(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnResponseReceived(MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        private async Task OnRequestInterceptedAsync(MessageEventArgs e)
        {
            if (e.AuthChallenge)
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
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object>
                {
                    {"interceptionId", e.InterceptionId},
                    {"authChallengeResponse", new { response, credentials.Username, credentials.Password }}
                });
                return;
            }
            if (!_userRequestInterceptionEnabled && _protocolRequestInterceptionEnabled)
            {
                await _client.SendAsync("Network.continueInterceptedRequest", new Dictionary<string, object> {
                    { "interceptionId", e.InterceptionId}
                });
            }

            if (!string.IsNullOrEmpty(e.RedirectUrl))
            {
                Contract.Ensures(_interceptionIdToRequest.ContainsKey(e.InterceptionId),
                                 "INTERNAL ERROR: failed to find request for interception redirect.");

                var request = _interceptionIdToRequest[e.InterceptionId];

                HandleRequestRedirect(request, e.ResponseStatusCode, e.ResponseHeaders);
                HandleRequestStart(request.RequestId, e.InterceptionId, e.RedirectUrl, e.ResourceType, e.Request);
                return;
            }
            var requestHash = GenerateRequestHash(e.Request);


            if (_requestHashToRequestIds.Any(i => i.Key == requestHash))
            {
                var item = _requestHashToRequestIds.FirstOrDefault(i => i.Key == requestHash);
                var requestId = item.Value;
                _requestHashToRequestIds.Remove(item);
                HandleRequestStart(requestId, e.InterceptionId, e.Request.Url, e.ResourceType, e.Request);
            }
            else
            {
                _requestHashToInterceptionIds.Add(requestHash, e.InterceptionId);
                HandleRequestStart(null, e.InterceptionId, e.Request.Url, e.ResourceType, e.Request);
            }
        }

        private string GenerateRequestHash(RequestData request)
        {
            throw new NotImplementedException();
        }

        private void HandleRequestStart(string requestId, string interceptionId, string redirectUrl, string resourceType, RequestData request)
        {
            throw new NotImplementedException();
        }

        private void HandleRequestRedirect(Request request, HttpStatusCode responseStatusCode, Dictionary<string, object> responseHeaders)
        {
            throw new NotImplementedException();
        }

        private void OnRequestWillBeSent(MessageEventArgs e)
        {
            throw new NotImplementedException();
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
