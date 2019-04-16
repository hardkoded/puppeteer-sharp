namespace PuppeteerSharp.Abstractions.Mobile
{
    /// <summary>
    /// Device descriptor.
    /// </summary>
    public class DeviceDescriptor
    {
        /// <summary>
        /// Device name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
        /// <summary>
        /// User Agent
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; set; }
        /// <summary>
        /// ViewPort.
        /// </summary>
        /// <value>The view port.</value>
        public ViewPortOptions ViewPort { get; set; }
    }
}