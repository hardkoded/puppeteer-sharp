namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PWAGetOsAppStateResponse
    {
        public int BadgeCount { get; set; }

        public PWAFileHandler[] FileHandlers { get; set; }
    }
}
