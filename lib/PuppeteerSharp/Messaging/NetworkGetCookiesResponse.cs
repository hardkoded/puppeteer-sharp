using PuppeteerSharp.Abstractions;

namespace PuppeteerSharp.Messaging
{
    internal class NetworkGetCookiesResponse
    {
        public CookieParam[] Cookies { get; set; }
    }
}
