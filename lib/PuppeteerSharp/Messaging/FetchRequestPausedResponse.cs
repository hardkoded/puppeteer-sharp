using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp.Messaging
{
    internal class FetchRequestPausedResponse
    {
        public string RequestId { get; set; }
        public string NetworkId { get; set; }
        public ResourceType ResourceType { get; set; }
        public HttpStatusCode ResponseStatusCode { get; set; }
        public Header[] ResponseHeaders { get; set; }
    }
}