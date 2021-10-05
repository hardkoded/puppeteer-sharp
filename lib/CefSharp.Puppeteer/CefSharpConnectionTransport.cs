using System;
using System.IO;
using System.Threading.Tasks;
using CefSharp.Callback;
using CefSharp.Internals;
using CefSharp.Puppeteer.Transport;

namespace CefSharp.Puppeteer
{
    internal class CefSharpConnectionTransport : IConnectionTransport
    {
        private readonly IBrowserHost _browserHost;
        private IDevToolsMessageObserver _devtoolsMessageObserver;
        private IRegistration _devtoolsRegistration;
        public bool IsClosed { get; private set; }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public CefSharpConnectionTransport(IBrowserHost browserHost)
        {
            _browserHost = browserHost;

            var observer = new CefSharpDevMessageObserver();
            observer.OnDevToolsAgentDetached((b) => { IsClosed = true; });
            observer.OnDevToolsMessage((b, m) =>
            {
                using var reader = new StreamReader(m);
                var msg = reader.ReadToEnd();
                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(msg));
            });

            _devtoolsMessageObserver = observer;

            _devtoolsRegistration = _browserHost.AddDevToolsMessageObserver(_devtoolsMessageObserver);
        }

        void IDisposable.Dispose()
        {
            _devtoolsRegistration?.Dispose();
            _devtoolsRegistration = null;
            _devtoolsMessageObserver = null;
        }

        public Task SendAsync(string message)
        {
            return CefThread.ExecuteOnUiThread(() =>
            {
                var result = _browserHost.SendDevToolsMessage(message);

                return result;
            });
        }

        public void StopReading()
        {
            _devtoolsRegistration?.Dispose();
            _devtoolsRegistration = null;
        }
    }
}
