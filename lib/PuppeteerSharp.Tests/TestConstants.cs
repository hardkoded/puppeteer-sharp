using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Mobile;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const string TestFixtureCollectionName = "PuppeteerLoaderFixture collection";
        public const int DebuggerAttachedTestTimeout = 300_000;
        public const int DefaultTestTimeout = 60_000;
        public const int DefaultPuppeteerTimeout = 10_000;
        public const int Port = 8081;
        public const int HttpsPort = Port + 1;
        public const string ServerUrl = "http://localhost:8081";
        public const string ServerIpUrl = "http://127.0.0.1:8081";
        public const string HttpsPrefix = "https://localhost:8082";
        public const string AboutBlank = "about:blank";
        public static readonly string CrossProcessHttpPrefix = "http://127.0.0.1:8081";
        public static readonly string EmptyPage = $"{ServerUrl}/empty.html";
        public static readonly string CrossProcessUrl = ServerIpUrl;
        public static readonly string ExtensionPath = Path.Combine(AppContext.BaseDirectory, "Assets", "simple-extension");
        public static readonly bool IsChrome = Environment.GetEnvironmentVariable("PRODUCT") != "FIREFOX";

        public static readonly DeviceDescriptor IPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
        public static readonly DeviceDescriptor IPhone6Landscape = Puppeteer.Devices[DeviceDescriptorName.IPhone6Landscape];

        public static ILoggerFactory LoggerFactory { get; private set; }
        public static string FileToUpload => Path.Combine(AppContext.BaseDirectory, "Assets", "file-to-upload.txt");

        public static readonly IEnumerable<string> NestedFramesDumpResult = new List<string>()
        {
            "http://localhost:<PORT>/frames/nested-frames.html",
            "    http://localhost:<PORT>/frames/two-frames.html (2frames)",
            "        http://localhost:<PORT>/frames/frame.html (uno)",
            "        http://localhost:<PORT>/frames/frame.html (dos)",
            "    http://localhost:<PORT>/frames/frame.html (aframe)"
        };

        public static LaunchOptions DefaultBrowserOptions() => new LaunchOptions
        {
            SlowMo = Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO")),
            Headless = Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true"),
            Product = IsChrome ? Product.Chrome : Product.Firefox,
            EnqueueAsyncMessages = Convert.ToBoolean(Environment.GetEnvironmentVariable("ENQUEUE_ASYNC_MESSAGES") ?? "false"),
            Timeout = 0,
            LogProcess = true,
#if NETCOREAPP
            EnqueueTransportMessages = false
#else
            EnqueueTransportMessages = true
#endif
        };

        public static LaunchOptions BrowserWithExtensionOptions() => new LaunchOptions
        {
            Headless = false,
            Args = new[]
            {
                $"--disable-extensions-except={ExtensionPath.Quote()}",
                $"--load-extension={ExtensionPath.Quote()}"
            }
        };
    }
}
