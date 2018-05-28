using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CSSStyleSheetAddedResponse
    {
        [JsonProperty("header")]
        public CSSStyleSheetAddedResponseHeader Header { get; set; }

        public class CSSStyleSheetAddedResponseHeader
        {
            [JsonProperty("styleSheetId")]
            public string StyleSheetId { get; set; }
            [JsonProperty("sourceURL")]
            public string SourceURL { get; set; }
        }
    }
}
