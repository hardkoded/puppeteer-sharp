namespace PuppeteerSharp.Mobile
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
        public string Name { get; internal set; }

        /// <summary>
        /// User Agent.
        /// </summary>
        /// <value>The user agent.</value>
        public string UserAgent { get; internal set; }

        /// <summary>
        /// ViewPort.
        /// </summary>
        /// <value>The view port.</value>
        public ViewPortOptions ViewPort { get; internal set; }
    }
}
