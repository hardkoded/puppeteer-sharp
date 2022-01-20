using System.Collections.Generic;
using CefSharp.Puppeteer.Mobile;

namespace CefSharp.Puppeteer
{
    /// <summary>
    /// Device Emulation
    /// </summary>
    public static class Emulation
    {
        /// <summary>
        /// Returns a list of devices to be used with <seealso cref="DevToolsContext.EmulateAsync(DeviceDescriptor)"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var iPhone = Emulation.Devices[DeviceDescriptorName.IPhone6];
        /// await devToolsContext.EmulateAsync(iPhone);
        /// await devToolsContext.goto('https://www.google.com');
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> Devices => DeviceDescriptors.ToReadOnly();

        /// <summary>
        /// Returns a list of network conditions to be used with <seealso cref="DevToolsContext.EmulateNetworkConditionsAsync(NetworkConditions)"/>.
        /// Actual list of conditions can be found in <seealso cref="PredefinedNetworkConditions.Conditions"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var slow3G = Emulation.NetworkConditions["Slow 3G"];
        /// await devToolsContext.EmulateNetworkConditionsAsync(slow3G);
        /// await devToolsContext.goto('https://www.google.com');
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<string, NetworkConditions> NetworkConditions => PredefinedNetworkConditions.ToReadOnly();
    }
}
