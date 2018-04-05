using System;

namespace PuppeteerSharp
{
    public class ErrorEventArgs : EventArgs
    {
        public string Error { get; }

        public ErrorEventArgs(string error)
        {
            Error = error;
        }
    }
}