using System;

namespace PuppeteerSharp
{
    public class PageErrorEventArgs : EventArgs
    {
        public PageError Error { get; set; }

        public PageErrorEventArgs(PageError error) => Error = error;
    }
}