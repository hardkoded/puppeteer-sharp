using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CefSharp.Puppeteer
{
    internal class MessageTask
    {
        internal MessageTask(string method)
        {
            Method = method;
            TaskWrapper = new TaskCompletionSource<JObject>(TaskContinuationOptions.RunContinuationsAsynchronously);
        }

        #region public Properties
        internal TaskCompletionSource<JObject> TaskWrapper { get; }

        internal string Method { get; }
        #endregion
    }
}
