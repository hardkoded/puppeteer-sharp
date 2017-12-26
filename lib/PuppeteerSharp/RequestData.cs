using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class RequestData
    {
        public string Url { get; set; }
        public string Method { get; set; }
        public Dictionary<string, object> PostData { get; set; }
        public Dictionary<string, object> Headers { get; set; }
    }
}