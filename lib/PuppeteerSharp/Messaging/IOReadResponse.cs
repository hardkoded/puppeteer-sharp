using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class IOReadResponse
    {
        [JsonProperty("eof")]
        internal bool Eof { get; set; }
        [JsonProperty(Constants.DATA)]
        internal string Data { get; set; }
    }
}
