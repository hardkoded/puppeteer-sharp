using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CSSStyleSheetAddedResponse
    {
        [JsonProperty(Constants.HEADER)]
        public CSSStyleSheetAddedResponseHeader Header { get; set; }

        public class CSSStyleSheetAddedResponseHeader
        {
            [JsonProperty(Constants.STYLE_SHEET_ID)]
            public string StyleSheetId { get; set; }
            [JsonProperty(Constants.SOURCE_URL)]
            public string SourceURL { get; set; }
        }
    }
}
