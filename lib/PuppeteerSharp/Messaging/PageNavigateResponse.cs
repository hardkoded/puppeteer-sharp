using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageNavigateResponse
    {
        [JsonProperty("errorText")]
        internal string ErrorText { get; set; }

        [JsonProperty("loaderId")]
        internal string LoaderId { get; set; }
    }
}
