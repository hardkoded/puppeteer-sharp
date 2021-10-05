using Newtonsoft.Json;

namespace CefSharp.Puppeteer.Messaging
{
    internal class PageHandleJavaScriptDialogRequest
    {
        public bool Accept { get; set; }

        public string PromptText { get; set; }
    }
}
