using System;
using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class NetworkGetResponseBodyResponse
    {
        [JsonProperty(Constants.BODY)]
        public string Body { get; set; }
        [JsonProperty(Constants.BASE_64_ENCODED)]
        public bool Base64Encoded { get; set; }
    }
}
