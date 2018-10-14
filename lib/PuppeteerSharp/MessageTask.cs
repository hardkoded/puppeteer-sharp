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

        #region public Properties
        internal TaskCompletionSource<JObject> TaskWrapper { get; set; }
        internal string Method { get; set; }
        #endregion
    }
}
