using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp
{
    internal class MessageTask
    {
        internal MessageTask()
        {
        }

        internal string Message { get; set; }

        internal TaskCompletionSource<JObject> TaskWrapper { get; set; }

        internal string Method { get; set; }
    }
}
