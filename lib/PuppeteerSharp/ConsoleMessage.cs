using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class ConsoleMessage
    {
        public ConsoleType Type { get; }
        public IList<JSHandle> Args { get; }
        
        private static readonly Dictionary<string, ConsoleType> ConsoleTypeMap = new Dictionary<string, ConsoleType>
        {
            ["log"] = ConsoleType.Log,
            ["debug"] = ConsoleType.Debug,
            ["trace"] = ConsoleType.Trace,
            ["dir"] = ConsoleType.Dir,
            ["warning"] = ConsoleType.Warning,
            ["error"] = ConsoleType.Error,
            ["time"] = ConsoleType.Time,
            ["timeEnd"] = ConsoleType.TimeEnd,
        };

        public ConsoleMessage(string type, IList<JSHandle> args)
        {
            Type = GetConsoleType(type);
            Args = args;
        }

        public string Text()
        {
            return string.Join(" ", Args);
        }

        private static ConsoleType GetConsoleType(string consoleType)
        {
            if (ConsoleTypeMap.ContainsKey(consoleType))
            {
                return ConsoleTypeMap[consoleType];
            }

            throw new PuppeteerException($"Unknown javascript console type {consoleType}");
        }
    }
}
