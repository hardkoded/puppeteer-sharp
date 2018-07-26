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
            [JsonProperty("args")]
            public List<dynamic> Args { get; internal set; }
            [JsonProperty("level")]
            internal ConsoleType Level { get; set; }
            [JsonProperty("text")]
            internal string Text { get; set; }
        }
    }
}