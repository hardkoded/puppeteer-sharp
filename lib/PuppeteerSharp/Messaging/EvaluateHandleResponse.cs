using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.Messaging
{
    internal class EvaluateHandleResponse
    {
        public EvaluateExceptionResponseDetails ExceptionDetails { get; set; }
        public RemoteObject Result { get; set; }
    }
}
