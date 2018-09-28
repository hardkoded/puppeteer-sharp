using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class LogEntryAddedResponse
    {
        [JsonProperty(Constants.ENTRY)]
        internal LogEntry Entry { get; set; }

        internal class LogEntry
        {
            [JsonProperty(Constants.SOURCE)]
            public TargetType Source { get; set; }
            [JsonProperty(Constants.ARGS)]
            internal dynamic[] Args { get; set; }
            [JsonProperty(Constants.LEVEL)]
            internal ConsoleType Level { get; set; }
            [JsonProperty(Constants.TEXT)]
            internal string Text { get; set; }
        }
    }
}