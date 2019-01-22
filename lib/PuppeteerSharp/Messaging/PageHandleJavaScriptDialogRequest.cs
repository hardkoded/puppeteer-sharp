using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageHandleJavaScriptDialogRequest
    {
        public bool Accept { get; set; }

        public string PromptText { get; set; }
    }
}
