using System.Collections.Generic;
using System.Net;

namespace PuppeteerSharp
{
    public class MessageEventArgs
    {
        public string MessageID { get; set; }
        public dynamic MessageData { get; set; }
    }

}