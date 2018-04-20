using System;

namespace PuppeteerSharp
{
    public class ConsoleEventArgs : EventArgs
    {
        public ConsoleMessage Message { get; }

        public ConsoleEventArgs(ConsoleMessage message) => Message = message;
    }
}