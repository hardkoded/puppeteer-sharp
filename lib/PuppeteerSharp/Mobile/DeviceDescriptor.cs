namespace PuppeteerSharp.Mobile
{
    public class DeviceDescriptor
    {
        public string Name { get; internal set; }
        public string UserAgent { get; internal set; }
        public ViewPortOptions ViewPort { get; internal set; }
    }
}