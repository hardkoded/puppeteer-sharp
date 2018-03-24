using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class CookieParam
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("domain", NullValueHandling = NullValueHandling.Ignore)]
        public string Domain { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; internal set; }

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty("expires", NullValueHandling = NullValueHandling.Ignore)]
        public int? Expires { get; set; }

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        public int? Size { get; set; }

        [JsonProperty("httpOnly", NullValueHandling = NullValueHandling.Ignore)]
        public bool? HttpOnly { get; set; }

        [JsonProperty("secure", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Secure { get; set; }

        [JsonProperty("session", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Session { get; set; }
    }
}