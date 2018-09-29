using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetNavigationHistoryResponse
    {
        [JsonProperty("currentIndex")]
        public int CurrentIndex { get; set; }
        [JsonProperty("entries")]
        public List<HistoryEntry> Entries { get; set; }

        internal class HistoryEntry
        {
            [JsonProperty("id")]
            internal int Id { get; set; }
        }
    }
}
