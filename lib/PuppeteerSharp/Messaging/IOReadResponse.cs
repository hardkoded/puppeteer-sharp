using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class IOReadResponse
    {
        [JsonProperty("eof")]
        internal bool Eof { get; set; }
        [JsonProperty("data")]
        internal string Data { get; set; }
    }
}
