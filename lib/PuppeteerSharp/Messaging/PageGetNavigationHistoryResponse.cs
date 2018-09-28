using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetNavigationHistoryResponse
    {
        [JsonProperty(Constants.CURRENT_INDEX)]
        public int CurrentIndex { get; set; }
        [JsonProperty(Constants.ENTRIES)]
        public List<HistoryEntry> Entries { get; set; }

        internal class HistoryEntry
        {
            [JsonProperty(Constants.ID)]
            internal int Id { get; set; }
        }
    }
}
