namespace PuppeteerSharp.Mobile
{
    /// <summary>
    /// Device descriptor.
    /// </summary>
    [System.Obsolete("Use PuppeteerSharp.Abstractions.Mobile.DeviceDescriptor class instead")]
    public class DeviceDescriptor : Abstractions.Mobile.DeviceDescriptor
    {

        /// <summary>
        /// ViewPort.
        /// </summary>
        /// <value>The view port.</value>
        public new ViewPortOptions ViewPort { get; set; }
    }
}