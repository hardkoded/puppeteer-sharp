namespace PuppeteerSharp.Cdp.Messaging
{
    internal class LogEntryAddedResponse
    {
        public LogEntry Entry { get; set; }

        internal class LogEntry
        {
            public TargetType Source { get; set; }

            public RemoteObject[] Args { get; set; }

            public ConsoleType Level { get; set; }

            public string Text { get; set; }

            public string URL { get; set; }

            public int? LineNumber { get; set; }
        }
    }
}
