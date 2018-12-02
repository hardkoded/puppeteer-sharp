namespace PuppeteerSharp.Messaging
{
    internal class LogEntryAddedResponse
    {
        public LogEntry Entry { get; set; }

        internal class LogEntry
        {
            public TargetType Source { get; set; }
            public dynamic[] Args { get; set; }
            public ConsoleType Level { get; set; }
            public string Text { get; set; }
        }
    }
}