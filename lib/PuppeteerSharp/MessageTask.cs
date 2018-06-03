using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    internal class MessageTask
    {
        internal MessageTask()
        {
        }

        #region public Properties
        internal TaskCompletionSource<dynamic> TaskWrapper { get; set; }
        internal string Method { get; set; }
        internal bool RawContent { get; set; }
        #endregion
    }
}
