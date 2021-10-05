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
        /// Returns a list of devices to be used with <seealso cref="Page.EmulateAsync(DeviceDescriptor)"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var iPhone = Emulation.Devices[DeviceDescriptorName.IPhone6];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateAsync(iPhone);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> Devices => DeviceDescriptors.ToReadOnly();

        /// <summary>
        /// Returns a list of network conditions to be used with <seealso cref="Page.EmulateNetworkConditionsAsync(NetworkConditions)"/>.
        /// Actual list of conditions can be found in <seealso cref="PredefinedNetworkConditions.Conditions"/>.
        /// </summary>
        /// <example>
        /// <code>
        ///<![CDATA[
        /// var slow3G = Emulation.NetworkConditions["Slow 3G"];
        /// using(var page = await browser.NewPageAsync())
        /// {
        ///     await page.EmulateNetworkConditionsAsync(slow3G);
        ///     await page.goto('https://www.google.com');
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IReadOnlyDictionary<string, NetworkConditions> NetworkConditions => PredefinedNetworkConditions.ToReadOnly();
    }
}
