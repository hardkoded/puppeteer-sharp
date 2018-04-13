using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class ConsoleMessage
    {
        public ConsoleType Type { get; }
        public IList<JSHandle> Args { get; }

        public ConsoleMessage(ConsoleType type, IList<JSHandle> args)
        {
            Type = type;
            Args = args;
        }

        public string Text() => string.Join(" ", Args);
    }
}
