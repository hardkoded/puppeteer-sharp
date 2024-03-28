namespace PuppeteerSharp.Cdp.Messaging
{
    internal class NetworkSetCacheDisabledRequest(bool cacheDisabled)
    {
        public bool CacheDisabled { get; set; } = cacheDisabled;
    }
}
