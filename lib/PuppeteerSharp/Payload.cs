using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class Payload
    {
        public string Method { get; internal set; }
        public object PostData { get; internal set; }
        public Dictionary<string, object> Headers { get; internal set; }
    }
}