using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace PuppeteerSharp
{
    public class Payload
    {
        public Payload()
        {
            Headers = new Dictionary<string, object>();
        }

        [JsonProperty("method")]
        public string Method { get; internal set; }
        public object PostData { get; internal set; }
        [JsonProperty("headers")]
        public Dictionary<string, object> Headers { get; internal set; }
        [JsonProperty("url")]
        public string Url { get; internal set; }

        [JsonIgnore]
        public string Hash
        {
            get
            {
                var normalizedUrl = Url;

                try
                {
                    // Decoding is necessary to normalize URLs.
                    // The method will throw if the URL is malformed. In this case,
                    // consider URL to be normalized as-is.
                    normalizedUrl = HttpUtility.UrlDecode(Url);
                }
                catch { }

                var hash = new Payload()
                {
                    Url = Url,
                    Method = Method,
                    PostData = PostData
                };

                if (!normalizedUrl.StartsWith("data:", StringComparison.Ordinal))
                {
                    foreach (var item in Headers.Where(kv => kv.Key != "Accept" && kv.Key != "Referrer" &&
                                                               kv.Key != "X-DevTools-Emulate-Network-Conditions-Client-Id"))
                    {
                        hash.Headers[item.Key] = item.Value;
                    }
                }

                return JsonConvert.SerializeObject(this);
            }
        }
    }
}