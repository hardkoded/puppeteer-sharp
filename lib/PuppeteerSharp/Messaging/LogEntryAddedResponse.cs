using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LogEntryAddedResponse
    {
        [JsonProperty("entry")]
        internal LogEntry Entry { get; set; }

        internal class LogEntry
        {
            [JsonProperty("source")]
            public TargetType Source { get; set; }
            [JsonProperty("args")]
            internal dynamic[] Args { get; set; }
            [JsonProperty("level")]
            internal ConsoleType Level { get; set; }
            [JsonProperty("text")]
            internal string Text { get; set; }
        }
    }
}