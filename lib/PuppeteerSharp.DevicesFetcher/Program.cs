using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PuppeteerSharp.DevicesFetcher
{
    class Program
    {
        const string DEVICES_URL = "https://raw.githubusercontent.com/ChromeDevTools/devtools-frontend/master/front_end/emulated_devices/module.json";
        const string HELP_MESSAGE = @"Usage: dotnet run [-u <from>] <outputPath>
  -u, --url    The URL to load devices descriptor from. If not set,
               devices will be fetched from the tip-of-tree of DevTools
               frontend.

  -h, --help   Show this help message

Fetch Chrome DevTools front-end emulation devices from given URL, convert them to puppeteer
devices and save to the <outputPath>.
";
        const string TEMPLATE = @"/**
 * Copyright 2017 Google Inc. All rights reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the ""License"");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an ""AS IS"" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

module.exports = {0};
for (let device of module.exports)
  module.exports[device.name] = device;
";
        static string outputPath;

        static void Main(string[] args)
        {
            var url = DEVICES_URL ?? args[0];
            outputPath = "./DeviceDescriptors.js" ?? args[1];

            MainAsync(url).GetAwaiter().GetResult();
        }

        static async Task MainAsync(string url)
        {
            string chromeVersion;
            using (var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
            {
                ExecutablePath = @"../PuppeteerSharp.Tests/bin/Debug/netcoreapp2.0/.local-chromium/Win64-571375/chrome-win32/chrome.exe"
            }))
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
                    if (landscape.viewport.width != device.viewport.width || landscape.viewport.height != device.viewport.height)
                        devices.Add(landscape);
                }
            }
            devices.RemoveAll(device => !device.viewport.isMobile);
            devices.Sort((a, b) => a.name.CompareTo(b.name));
            File.WriteAllText(outputPath, string.Format(TEMPLATE, JsonConvert.SerializeObject(devices, Formatting.Indented))
                .Replace('/', '\\')
                .Replace('"', '\''));
        }

        static OutputDevice CreateDevice(string chromeVersion, string deviceName, RootObject.Device descriptor, bool landscape)
        {
            var devicePayload = LoadFromJSONV1(descriptor);
            var viewportPayload = landscape ? devicePayload.horizontal : devicePayload.vertical;
            return new OutputDevice
            {
                name = deviceName + (landscape ? " landscape" : string.Empty),
                userAgent = devicePayload.userAgent.Replace("%s", chromeVersion),
                viewport = new OutputDevice.OutputDeviceViewport
                {
                    width = viewportPayload.width,
                    height = viewportPayload.height,
                    deviceScaleFactor = devicePayload.deviceScaleFactor,
                    isMobile = devicePayload.capabilities.Contains("mobile"),
                    hasTouch = devicePayload.capabilities.Contains("touch"),
                    isLandscape = landscape
                }
            };
        }

        static DevicePayload LoadFromJSONV1(RootObject.Device json) => new DevicePayload
        {
            type = json.type,
            userAgent = json.UserAgent,
            capabilities = json.Capabilities.ToHashSet(),
            deviceScaleFactor = json.Screen.DevicePixelRatio,
            horizontal = new DevicePayload.ViewportPayload
            {
                height = json.Screen.horizontal.height,
                width = json.Screen.horizontal.width
            },
            vertical = new DevicePayload.ViewportPayload
            {
                height = json.Screen.vertical.height,
                width = json.Screen.vertical.width
            }
        };

        static Task<string> HttpGET(string url) => new HttpClient().GetStringAsync(url);
    }

    public class DevicePayload
    {
        public string type { get; set; }
        public string userAgent { get; set; }
        public ViewportPayload vertical { get; set; }
        public ViewportPayload horizontal { get; set; }

        public double deviceScaleFactor { get; set; }
        public HashSet<string> capabilities { get; set; }

        public class ViewportPayload
        {
            public double width { get; set; }
            public double height { get; set; }
        }
    }

    public class OutputDevice
    {
        public string name { get; set; }
        public string userAgent { get; set; }
        public OutputDeviceViewport viewport { get; set; }

        public class OutputDeviceViewport
        {
            public double width { get; set; }
            public double height { get; set; }
            public double deviceScaleFactor { get; set; }
            public bool isMobile { get; set; }
            public bool hasTouch { get; set; }
            public bool isLandscape { get; set; }
        }
    }

    public class RootObject
    {
        [JsonProperty("extensions")]
        public Extension[] Extensions { get; set; }

        public class Extension
        {
            [JsonProperty("type")]
            public string Type { get; set; }
            [JsonProperty("device")]
            public Device Device { get; set; }
        }

        public class Device
        {
            [JsonProperty("show-by-default")]
            public bool ShowByDefault { get; set; }
            [JsonProperty("title")]
            public string Title { get; set; }
            [JsonProperty("screen")]
            public Screen Screen { get; set; }
            [JsonProperty("capabilities")]
            public string[] Capabilities { get; set; }
            [JsonProperty("user-agent")]
            public string UserAgent { get; set; }
            [JsonProperty("type")]
            public string type { get; set; }
            [JsonProperty("modes")]
            public Mode[] Modes { get; set; }
        }

        public class Screen
        {
            public Horizontal horizontal { get; set; }
            [JsonProperty("device-pixel-ratio")]
            public double DevicePixelRatio { get; set; }
            public Vertical vertical { get; set; }
        }

        public class Horizontal
        {
            public int width { get; set; }
            public int height { get; set; }
            public Outline outline { get; set; }
        }

        public class Outline
        {
            public string image { get; set; }
            public Insets insets { get; set; }
        }

        public class Insets
        {
            public int left { get; set; }
            public int top { get; set; }
            public int right { get; set; }
            public int bottom { get; set; }
        }

        public class Vertical
        {
            public int width { get; set; }
            public int height { get; set; }
            public Outline outline { get; set; }
        }

        public class Mode
        {
            public string title { get; set; }
            public string orientation { get; set; }
            public Insets insets { get; set; }
            public string image { get; set; }
        }
    }

}
