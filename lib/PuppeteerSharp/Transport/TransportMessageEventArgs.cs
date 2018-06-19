using System;

namespace PuppeteerSharp.Transport
{
    internal class TransportMessageEventArgs : EventArgs
    {
        internal string Response { get; }

        internal TransportMessageEventArgs(string response)
        {
            Response = response;
        }
    }
}