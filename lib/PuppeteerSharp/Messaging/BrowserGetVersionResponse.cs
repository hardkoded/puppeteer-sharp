using System.Threading.Tasks;

namespace PuppeteerSharp.Messaging
{
    internal class BrowserGetVersionResponse
    {
        public string UserAgent { get; set; }
        public string Product { get; set; }
    }
}
