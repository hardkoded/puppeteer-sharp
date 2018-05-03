using System;

namespace PuppeteerSharp
{
    public class PageErrorEventArgs : EventArgs
    {
        public string Message { get; set; }

        public PageErrorEventArgs(string message) => Message = message;
    }
}