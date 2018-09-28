using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CertificateErrorResponse
    {
        [JsonProperty(Constants.EVENT_ID)]
        public int EventId { get; set; }
    }
}