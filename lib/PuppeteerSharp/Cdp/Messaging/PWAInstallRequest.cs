namespace PuppeteerSharp.Cdp.Messaging
{
    internal class PWAInstallRequest
    {
        public string ManifestId { get; set; }

        public string InstallUrlOrBundleUrl { get; set; }
    }
}
