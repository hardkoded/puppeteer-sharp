using System.Collections.Generic;
namespace PuppeteerSharp.Messaging
{
    internal class FetchFailRequest
    {
        public string RequestId { get; set; }
        public string ErrorReason { get; set; }
    }
}
