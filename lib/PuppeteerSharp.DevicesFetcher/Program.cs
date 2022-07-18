using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace PuppeteerSharp.DevicesFetcher
{
    class Program
    {
        const string DEVICES_URL = "https://raw.githubusercontent.com/puppeteer/puppeteer/master/src/common/DeviceDescriptors.ts";

        static readonly string deviceDescriptorsOutput = "../../../../PuppeteerSharp/Mobile/DeviceDescriptors.cs";
        static readonly string deviceDescriptorNameOutput = "../../../../PuppeteerSharp/Mobile/DeviceDescriptorName.cs";

        static async Task Main(string[] args)
        {
            var url = DEVICES_URL;
            if (args.Length > 0)
            {
                url = args[0];
            }

            Console.WriteLine($"GET {url}");
            var text = await HttpGET(url);

            const string DeviceArray = "Device[] = [";
            var startIndex = text.IndexOf(DeviceArray) + DeviceArray.Length;
            var endIndex = text.IndexOf("];", startIndex);
            var length = endIndex - startIndex;
            text = "[" + text.Substring(startIndex, length) + "]";

            Device[] devices;
            try
            {
                devices = JsonConvert.DeserializeObject<Device[]>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: error parsing response - {ex.Message}");
                return;
            }

            WriteDeviceDescriptors(devices);
            WriteDeviceDescriptorName(devices);
        }

        static void WriteDeviceDescriptors(IEnumerable<Device> devices)
        {
            var builder = new StringBuilder();
            var begin = @"using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CefSharp.DevTools.Dom.Mobile
{
    /// <summary>
    /// Device descriptors.
    /// </summary>
    public static class DeviceDescriptors
    {
        private static readonly Dictionary<DeviceDescriptorName, DeviceDescriptor> Devices = new Dictionary<DeviceDescriptorName, DeviceDescriptor>
        {
";
            var end = @"
        };

        private static readonly Lazy<IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>> _readOnlyDevices =
            new Lazy<IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>>(() => new ReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor>(Devices));

        internal static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> ToReadOnly() => _readOnlyDevices.Value;
    }
}";

            builder.Append(begin);
            builder.AppendJoin(",\n", devices.Select(GenerateCsharpFromDevice));
            builder.Append(end);

            File.WriteAllText(deviceDescriptorsOutput, builder.ToString());
        }

        static void WriteDeviceDescriptorName(IEnumerable<Device> devices)
        {
            var builder = new StringBuilder();
            var begin = @"namespace CefSharp.DevTools.Dom.Mobile
{
    /// <summary>
    /// Device descriptor name.
    /// </summary>
    public enum DeviceDescriptorName
    {";
            var end = @"
    }
}";

            builder.Append(begin);
            builder.AppendJoin(",", devices.Select(device =>
            {
                return $@"
        /// <summary>
        /// {device.Name}
        /// </summary>
        {DeviceNameToEnumValue(device)}";
            }));
            builder.Append(end);

            File.WriteAllText(deviceDescriptorNameOutput, builder.ToString());
        }

        static string GenerateCsharpFromDevice(Device device)
        {
            return $@"            [DeviceDescriptorName.{DeviceNameToEnumValue(device)}] = new DeviceDescriptor
            {{
                Name = ""{device.Name}"",
                UserAgent = ""{device.UserAgent}"",
                ViewPort = new ViewPortOptions
                {{
                    Width = {device.Viewport.Width},
                    Height = {device.Viewport.Height},
                    DeviceScaleFactor = {device.Viewport.DeviceScaleFactor},
                    IsMobile = {device.Viewport.IsMobile.ToString().ToLower()},
                    HasTouch = {device.Viewport.HasTouch.ToString().ToLower()},
                    IsLandscape = {device.Viewport.IsLandscape.ToString().ToLower()}
                }}
            }}";
        }

        static string DeviceNameToEnumValue(Device device)
        {
            var dotNetName = device.Name.Replace("+", "Plus");
            var output = new StringBuilder();
            output.Append(char.ToUpper(dotNetName[0]));
            for (var i = 1; i < dotNetName.Length; i++)
            {
                if (char.IsWhiteSpace(dotNetName[i]))
                {
                    output.Append(char.ToUpper(dotNetName[i + 1]));
                    i++;
                }
                else
                {
                    output.Append(dotNetName[i]);
                }
            }

            return output.ToString();
        }

        static Task<string> HttpGET(string url) => new HttpClient().GetStringAsync(url);
    }
}
