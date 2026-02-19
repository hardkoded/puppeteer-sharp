namespace PuppeteerSharp.Cdp.Messaging
{
    internal class BrowserPermissionDescriptor
    {
        public string Name { get; set; }

        public bool? UserVisibleOnly { get; set; }

        public bool? Sysex { get; set; }

        public bool? PanTiltZoom { get; set; }

        public bool? AllowWithoutSanitization { get; set; }
    }
}
