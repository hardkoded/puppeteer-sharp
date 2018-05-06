using System;

namespace PuppeteerSharp
{
    [Serializable]
    public class SelectorException : PuppeteerException
    {
        public string Selector { get; }

        public SelectorException(string message) : base(message)
        {
        }

        public SelectorException(string message, string selector) : base(message)
        {
            Selector = selector;
        }
    }
}