using System;
namespace PuppeteerSharp
{
    public class NetworkManager
    {
        public NetworkManager(Session client)
        {
            _client = client;
        }

        #region Public Properties

        #endregion
        #region Private members
        private Session _client;
        #endregion
    }
}
