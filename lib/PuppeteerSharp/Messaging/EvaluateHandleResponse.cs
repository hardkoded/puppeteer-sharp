using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class EvaluateHandleResponse
    {
        public JToken ExceptionDetails { get; set; }
        public JToken Result { get; set; }
    }
}
