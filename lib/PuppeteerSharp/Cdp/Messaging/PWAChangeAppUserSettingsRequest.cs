namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PWAChangeAppUserSettingsRequest
    {
        public string ManifestId { get; set; }

        public PWADisplayMode? DisplayMode { get; set; }
    }
}
