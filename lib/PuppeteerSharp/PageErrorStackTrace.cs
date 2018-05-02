using System.Collections.Generic;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class PageErrorStackTrace
    {
        [JsonProperty("callFrames")]
        public List<PageErrorCallFrame> CallFrames { get; set; }
    }
}