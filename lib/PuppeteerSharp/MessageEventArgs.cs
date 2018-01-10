using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp
{
    public class MessageEventArgs
    {
        internal object eventId;
        internal object parentFrameId;

        public string MessageID { get; set; }
        public TargetInfo TargetInfo { get; set; }
        public bool AuthChallenge { get; internal set; }
        public string InterceptionId { get; internal set; }
        public string RedirectUrl { get; internal set; }
        public HttpStatusCode ResponseStatusCode { get; internal set; }
        public Dictionary<string, object> ResponseHeaders { get; internal set; }
        public string ResourceType { get; internal set; }
        public Payload Request { get; internal set; }
        public Response RedirectResponse { get; internal set; }
        public string RequestId { get; internal set; }
        public string Type { get; internal set; }
        public Response Response { get; internal set; }
        public string ErrorText { get; internal set; }
        public ExceptionInfo Exception { get; internal set; }
        public string FrameId { get; internal set; }
        public FrameData Frame { get; internal set; }
        public ContextData Context { get; internal set; }
        public string ExecutionContextId { get; internal set; }
        public string LoaderId { get; internal set; }
        public string Name { get; internal set; }
        public string ParentFrameId { get; internal set; }
    }

}