using System;
using System.Collections.Generic;
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
        private Dictionary<string, object> _requestHashToRequestIds = new Dictionary<string, object>();
        private Dictionary<string, object> _requestHashToInterceptionIds = new Dictionary<string, object>();
        #endregion

        public NetworkManager(Session client)
        {
            _client = client;
            _client.MessageReceived += client_MessageReceived;
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


        void client_MessageReceived(object sender, PuppeteerSharp.MessageEventArgs e)
        {

            switch (e.MessageID)
            {
                case "Network.requestWillBeSent":
                    OnRequestWillBeSent(e);
                    break;
                case "Network.requestIntercepted":
                    OnRequestIntercepted(e);
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

        private void OnRequestIntercepted(MessageEventArgs e)
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
