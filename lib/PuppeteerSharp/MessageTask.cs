using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CefSharp.DevTools.Dom
{
    internal class MessageTask
    {
        internal MessageTask(string method)
        {
            Method = method;
            TaskWrapper = new TaskCompletionSource<JObject>(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        internal TaskCompletionSource<JObject> TaskWrapper { get; }

        internal string Method { get; }
    }
}
