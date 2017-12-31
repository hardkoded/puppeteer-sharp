using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class NetworkManager
    {

        #region Private members
        private Session _client;
        private Dictionary<string, Request> _requestIdToRequest = new Dictionary<string, Request>();
        private Dictionary<string, Request> _interceptionIdToRequest = new Dictionary<string, Request>();
        private Dictionary<string, string> _extraHTTPHeaders = new Dictionary<string, string>();
        private bool _offine;
        private Credential _credentials;
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


        #endregion
    }
}
