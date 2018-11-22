using System.Collections.Generic;

namespace PuppeteerSharp.Messaging
{
    internal class PageGetNavigationHistoryResponse
    {
        public int CurrentIndex { get; set; }
        public List<HistoryEntry> Entries { get; set; }

        internal class HistoryEntry
        {
            public int Id { get; set; }
        }
    }
}
