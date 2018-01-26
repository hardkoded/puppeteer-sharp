using System;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class MessageTask
    {
        public MessageTask()
        {
        }

        #region public Properties
        public TaskCompletionSource<dynamic> TaskWrapper { get; set; }
        public string Method { get; set; }
        public bool RawContent { get; set; }
        #endregion
    }
}
