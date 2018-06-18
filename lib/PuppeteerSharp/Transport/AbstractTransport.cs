using System;
using System.Threading.Tasks;

namespace PuppeteerSharp.Transport
{
    internal abstract class AbstractTransport : IDisposable
    {
        #region Fields
        protected bool _stopListening;
        #endregion

        #region Properties
        internal bool IsClosed { get; set; }
        internal event EventHandler<TransportMessageEventArgs> OnMessage;
        internal event EventHandler Closed;
        #endregion

        #region Public Members
        public abstract void Dispose();
        #endregion
        #region Internal Members
        internal virtual void StopListening() => _stopListening = true;

        internal virtual void OnClose()
        {
            Closed?.Invoke(this, EventArgs.Empty);
        }

        protected void MessageReceived(string response)
        {
            OnMessage?.Invoke(this, new TransportMessageEventArgs(response));
        }

        internal virtual void Close()
        {
            IsClosed = true;
        }

        internal abstract void StartListening();
        internal abstract Task SendAsync(string message);

        #endregion
    }
}