using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class ConsoleMessage
    {
        public ConsoleType Type { get; }
        public string Text { get; }
        public IList<JSHandle> Args { get; }

        public ConsoleMessage(ConsoleType type, string text, IList<JSHandle> args)
        {
            Type = type;
            Text = text;
            Args = args;
        }
    }
}
