using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace PuppeteerSharp.DevicesFetcher
{
    class Program
    {
        const string DEVICES_URL = "https://raw.githubusercontent.com/ChromeDevTools/devtools-frontend/master/front_end/emulated_devices/module.json";

        static string deviceDescriptorsOutput = "../../../../PuppeteerSharp/Mobile/DeviceDescriptors.cs";
        static string deviceDescriptorNameOutput = "../../../../PuppeteerSharp/Mobile/DeviceDescriptorName.cs";

        static async Task Main(string[] args)
        {
            var url = DEVICES_URL;
            if (args.Length > 0)
            {
                url = args[0];
            }

            string chromeVersion = null;
            var fetcher = new BrowserFetcher();
            await fetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions()))
            {
                chromeVersion = (await browser.GetVersionAsync()).Split('/').Last();
            }

            Console.WriteLine($"GET {url}");
            var text = await HttpGET(url);
            RootObject json = null;
            try
            {
                json = JsonConvert.DeserializeObject<RootObject>(text);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAILED: error parsing response - {ex.Message}");
            }
            var devicePayloads = json.Extensions
                .Where(extension => extension.Type == "emulated-device")
                .Select(extension => extension.Device)
                .ToArray();
            var devices = new List<OutputDevice>();
            foreach (var payload in devicePayloads)
            {
                string[] names;
                if (payload.Title == "iPhone 6/7/8")
                {
                    names = new[] { "iPhone 6", "iPhone 7", "iPhone 8" };
                }
                else if (payload.Title == "iPhone 6/7/8 Plus")
                {
                    names = new[] { "iPhone 6 Plus", "iPhone 7 Plus", "iPhone 8 Plus" };
                }
                else if (payload.Title == "iPhone 5/SE")
                {
                    names = new[] { "iPhone 5", "iPhone SE" };
                }
                else
                {
                    names = new[] { payload.Title };
                }
                foreach (var name in names)
                {
                    var device = CreateDevice(chromeVersion, name, payload, false);
                    var landscape = CreateDevice(chromeVersion, name, payload, true);
                    devices.Add(device);
                    if (landscape.Viewport.Width != device.Viewport.Width || landscape.Viewport.Height != device.Viewport.Height)
                    {
                        devices.Add(landscape);
                    }
                }
            }
            devices.RemoveAll(device => !device.Viewport.IsMobile);
            devices.Sort((a, b) => a.Name.CompareTo(b.Name));

            WriteDeviceDescriptors(devices);
            WriteDeviceDescriptorName(devices);
        }

        static void WriteDeviceDescriptors(IEnumerable<OutputDevice> devices)
        {
            var builder = new StringBuilder();
            var begin = @"using System.Collections.Generic;

namespace PuppeteerSharp.Mobile
{
    /// <summary>
    /// Device descriptors.
    /// </summary>
    public class DeviceDescriptors
    {
        private static readonly Dictionary<DeviceDescriptorName, DeviceDescriptor> Devices = new Dictionary<DeviceDescriptorName, DeviceDescriptor>
        {
";
            var end = @"
        };
            
        /// <summary>
        /// Get the specified device description.
        /// </summary>
        /// <returns>The device descriptor.</returns>
        /// <param name=""name"">Device Name.</param>
        public static DeviceDescriptor Get(DeviceDescriptorName name) => Devices[name];
    }
}";

            builder.Append(begin);
            builder.AppendJoin(",\n", devices.Select(GenerateCsharpFromDevice));
            builder.Append(end);

            File.WriteAllText(deviceDescriptorsOutput, builder.ToString());
        }

        static void WriteDeviceDescriptorName(IEnumerable<OutputDevice> devices)
        {
            var builder = new StringBuilder();
            var begin = @"namespace PuppeteerSharp.Mobile
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

        static string GenerateCsharpFromDevice(OutputDevice device)
        {
            var w = string.Empty;
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
        static string DeviceNameToEnumValue(OutputDevice device)
        {
            var output = new StringBuilder();
            output.Append(char.ToUpper(device.Name[0]));
            for (var i = 1; i < device.Name.Length; i++)
            {
                if (char.IsWhiteSpace(device.Name[i]))
                {
                    output.Append(char.ToUpper(device.Name[i + 1]));
                    i++;
                }
                else
                {
                    output.Append(device.Name[i]);
                }
            }

            return output.ToString();
        }

        static OutputDevice CreateDevice(string chromeVersion, string deviceName, RootObject.Device descriptor, bool landscape)
        {
            var devicePayload = LoadFromJSONV1(descriptor);
            var viewportPayload = landscape ? devicePayload.Horizontal : devicePayload.Vertical;
            return new OutputDevice
            {
                Name = deviceName + (landscape ? " landscape" : string.Empty),
                UserAgent = devicePayload.UserAgent.Replace("%s", chromeVersion),
                Viewport = new OutputDevice.OutputDeviceViewport
                {
                    Width = viewportPayload.Width,
                    Height = viewportPayload.Height,
                    DeviceScaleFactor = devicePayload.DeviceScaleFactor,
                    IsMobile = devicePayload.Capabilities.Contains("mobile"),
                    HasTouch = devicePayload.Capabilities.Contains("touch"),
                    IsLandscape = landscape
                }
            };
        }

        static DevicePayload LoadFromJSONV1(RootObject.Device json) => new DevicePayload
        {
            Type = json.Type,
            UserAgent = json.UserAgent,
            Capabilities = json.Capabilities.ToHashSet(),
            DeviceScaleFactor = json.Screen.DevicePixelRatio,
            Horizontal = new ViewportPayload
            {
                Height = json.Screen.Horizontal.Height,
                Width = json.Screen.Horizontal.Width
            },
            Vertical = new ViewportPayload
            {
                Height = json.Screen.Vertical.Height,
                Width = json.Screen.Vertical.Width
            }
        };

        static Task<string> HttpGET(string url) => new HttpClient().GetStringAsync(url);
    }
}
