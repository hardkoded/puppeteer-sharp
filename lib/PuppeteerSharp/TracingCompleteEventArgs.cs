using System.IO;
namespace PuppeteerSharp
{
    public class TracingCompleteEventArgs
    {
        public Stream Stream { get; internal set; }
    }
}