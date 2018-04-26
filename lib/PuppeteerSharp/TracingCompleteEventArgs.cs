using System;
using System.IO;
namespace PuppeteerSharp
{
    public class TracingCompleteEventArgs : EventArgs
    {
        public string Stream { get; internal set; }
    }
}