using System;
using System.IO;
using System.Threading.Tasks;
using CefSharp.Callback;
using CefSharp.Dom.Transport;
using CefSharp.Internals;

namespace CefSharp.Dom
{
    /// <summary>
    /// CefSharpConnectionTransport used to send/receive messages
    /// to the <see cref="IBrowserHost"/>
    /// </summary>
    public sealed class CefSharpConnectionTransport : IConnectionTransport, IDevToolsMessageObserver
    {
        private readonly IBrowserHost _browserHost;

        /// <summary>
        /// The DevTools registration is disposed via a call to StopReading
        /// when we explicitly Dispose of the instance ourselves, otherwise
        /// it's called from within CEF when the browser is shutting down.
        /// </summary>
#pragma warning disable CA2213 // Disposable fields should be disposed
        private IRegistration _devtoolsRegistration;
#pragma warning restore CA2213 // Disposable fields should be disposed

        /// <inheritdoc/>
        public bool IsClosed { get; private set; }

        /// <inheritdoc/>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        /// <inheritdoc/>
        public event EventHandler<MessageErrorEventArgs> MessageError;
        /// <inheritdoc/>
        public event EventHandler Disconnected;

        /// <summary>
        /// Creates a new <see cref="CefSharpConnectionTransport"/> instance
        /// </summary>
        /// <param name="browserHost">browserHost</param>
        public CefSharpConnectionTransport(IBrowserHost browserHost)
        {
            _browserHost = browserHost;

            _devtoolsRegistration = _browserHost.AddDevToolsMessageObserver(this);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            MessageReceived = null;
            MessageError = null;
            Disconnected = null;
        }

        /// <inheritdoc/>
        public Task SendAsync(string message)
        {
            return CefThread.ExecuteOnUiThread(() =>
            {
                // For now do nothing if the BrowserHost is disposed
                // Issue https://github.com/cefsharp/CefSharp.Dom/issues/41
                if (_browserHost.IsDisposed)
                {
                    return false;
                }

                var result = _browserHost.SendDevToolsMessage(message);

                return result;
            });
        }

        /// <inheritdoc/>
        public void StopReading()
        {
            _devtoolsRegistration?.Dispose();
            _devtoolsRegistration = null;
        }

        /// <inheritdoc/>
        bool IDevToolsMessageObserver.OnDevToolsMessage(IBrowser browser, Stream message)
        {
            try
            {
                using var reader = new StreamReader(message);
                var msg = reader.ReadToEnd();
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));
            }
            catch (Exception ex)
            {
                MessageError?.Invoke(this, new MessageErrorEventArgs(ex));
            }

            return true;
        }

        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsMethodResult(IBrowser browser, int messageId, bool success, Stream result) => throw new NotImplementedException();
        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsEvent(IBrowser browser, string method, Stream parameters) => throw new NotImplementedException();
        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsAgentAttached(IBrowser browser)
        {
        }
        /// <inheritdoc/>
        void IDevToolsMessageObserver.OnDevToolsAgentDetached(IBrowser browser)
        {
            IsClosed = true;

            Disconnected?.Invoke(this, EventArgs.Empty);
            Disconnected = null;
        }
    }
}
