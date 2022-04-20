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

        internal TaskCompletionSource<JObject> TaskWrapper { get; set; }

        internal string Method { get; set; }
    }
}
