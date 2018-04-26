using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    public class IOReadResponse
    {
        [JsonProperty("eof")]
        public bool Eof { get; set; }
        [JsonProperty("data")]
        public string Data { get; set; }
    }
}
