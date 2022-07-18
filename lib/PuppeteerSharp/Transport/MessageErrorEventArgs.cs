using System;

namespace CefSharp.DevTools.Dom.Transport
{
    /// <summary>
    /// Message received event arguments.
    /// <see cref="IConnectionTransport.MessageReceived"/>
    /// </summary>
    public class MessageErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="ex">Exception</param>
        public MessageErrorEventArgs(Exception ex) => Exception = ex;
        /// <summary>
        /// Exception
        /// </summary>
        public Exception Exception { get; }
    }
}
