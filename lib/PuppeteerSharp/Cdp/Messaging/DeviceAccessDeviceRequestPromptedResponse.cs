namespace PuppeteerSharp.Cdp.Messaging
{
    internal class DeviceAccessDeviceRequestPromptedResponse
    {
        public string Id { get; set; }

        public DeviceAccessDevice[] Devices { get; set; } = [];

        internal class DeviceAccessDevice
        {
            public string Name { get; set; }

            public string Id { get; set; }
        }
    }
}
