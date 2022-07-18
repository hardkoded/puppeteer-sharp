using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom.Messaging
{
    internal class EvaluateHandleResponse
    {
        public EvaluateExceptionResponseDetails ExceptionDetails { get; set; }

        public RemoteObject Result { get; set; }
    }
}
