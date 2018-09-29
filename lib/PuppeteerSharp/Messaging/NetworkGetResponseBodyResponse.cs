using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class NetworkGetResponseBodyResponse
    {
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("base64Encoded")]
        public bool Base64Encoded { get; set; }
    }
}
