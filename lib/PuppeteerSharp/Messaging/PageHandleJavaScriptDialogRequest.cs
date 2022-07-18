using Newtonsoft.Json;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class PageHandleJavaScriptDialogRequest
    {
        public bool Accept { get; set; }

        public string PromptText { get; set; }
    }
}
