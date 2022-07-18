using System;
using System.IO;
using CefSharp.Callback;

namespace CefSharp.DevTools.Dom
{
    internal class CefSharpDevMessageObserver : IDevToolsMessageObserver
    {
        private Action<IBrowser, Stream> _onDevToolsMessageAction;
        private Action<IBrowser> _onDevtoolsAgentDetached;

        public void Dispose()
        {
        }

        void IDevToolsMessageObserver.OnDevToolsAgentAttached(IBrowser browser)
        {
        }

        public CefSharpDevMessageObserver OnDevToolsAgentDetached(Action<IBrowser> action)
        {
            _onDevtoolsAgentDetached = action;

            return this;
        }

        public void OnDevToolsAgentDetached(IBrowser browser)
        {
            _onDevtoolsAgentDetached?.Invoke(browser);
        }

        void IDevToolsMessageObserver.OnDevToolsEvent(IBrowser browser, string method, Stream parameters)
        {
            throw new NotImplementedException();
        }

        public CefSharpDevMessageObserver OnDevToolsMessage(Action<IBrowser, Stream> action)
        {
            _onDevToolsMessageAction = action;

            return this;
        }

        bool IDevToolsMessageObserver.OnDevToolsMessage(IBrowser browser, Stream message)
        {
            _onDevToolsMessageAction?.Invoke(browser, message);

            return true;
        }

        void IDevToolsMessageObserver.OnDevToolsMethodResult(IBrowser browser, int messageId, bool success, Stream result)
        {
            throw new NotImplementedException();
        }
    }
}
