using PuppeteerSharp.Abstractions;

namespace PuppeteerSharp.Messaging
{
    internal class NetworkSetCookiesRequest
    {
        public CookieParam[] Cookies { get; set; }
    }
}
