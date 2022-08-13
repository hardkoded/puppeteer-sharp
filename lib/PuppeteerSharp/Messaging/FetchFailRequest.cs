using System.Collections.Generic;

namespace CefSharp.Dom.Messaging
{
    internal class FetchFailRequest
    {
        public string RequestId { get; set; }

        public string ErrorReason { get; set; }
    }
}
