namespace PuppeteerSharp.Messaging
{
    internal class PageJavascriptDialogOpeningResponse
    {
        public DialogType Type { get; set; }
        public string DefaultPrompt { get; set; }
        public string Message { get; set; }
    }
}
