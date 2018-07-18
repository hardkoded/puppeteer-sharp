using Newtonsoft.Json;

namespace PuppeteerSharp.Messaging
{
    internal class CertificateErrorResponse
    {
        [JsonProperty("eventId")]
        public int EventId { get; set; }
    }
}