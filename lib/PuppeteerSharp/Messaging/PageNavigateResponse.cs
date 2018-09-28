using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class PageNavigateResponse
    {
        [JsonProperty(Constants.ERROR_TEXT)]
        internal string ErrorText { get; set; }

        [JsonProperty(Constants.LOADER_ID)]
        internal string LoaderId { get; set; }
    }
}
