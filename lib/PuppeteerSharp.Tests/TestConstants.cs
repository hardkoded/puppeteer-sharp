using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.TestServer;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const int DebuggerAttachedTestTimeout = 300_000;
        public const int DefaultPuppeteerTimeout = 10_000;
        public const string AboutBlank = "about:blank";

        public static int Port { get; private set; }
        public static int HttpsPort { get; private set; }
        public static string ServerUrl { get; private set; }
        public static string ServerIpUrl { get; private set; }
        public static string HttpsPrefix { get; private set; }
        public static string CrossProcessHttpPrefix { get; private set; }
        public static string CrossProcessHttpsPrefix { get; private set; }
        public static string EmptyPage { get; private set; }
        public static string CrossProcessUrl { get; private set; }
        public static readonly bool IsChrome = PuppeteerTestAttribute.IsChrome;
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

        public static void Initialize(SimpleServer server, SimpleServer httpsServer)
        {
            Port = server.Port;
            HttpsPort = httpsServer.Port;
            ServerUrl = server.Prefix;
            ServerIpUrl = server.IpPrefix;
            HttpsPrefix = httpsServer.Prefix;
            CrossProcessHttpPrefix = server.IpPrefix;
            CrossProcessHttpsPrefix = httpsServer.IpPrefix;
            EmptyPage = $"{ServerUrl}/empty.html";
            CrossProcessUrl = ServerIpUrl;
        }

        public static LaunchOptions DefaultBrowserOptions() => new()
        {
            SlowMo = Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO")),
            HeadlessMode = PuppeteerTestAttribute.Headless,
            Browser = IsChrome ? SupportedBrowser.Chrome : SupportedBrowser.Firefox,
            EnqueueAsyncMessages = Convert.ToBoolean(Environment.GetEnvironmentVariable("ENQUEUE_ASYNC_MESSAGES") ?? "false"),
            Timeout = 0,
            Protocol = PuppeteerTestAttribute.IsCdp ? ProtocolType.Cdp : ProtocolType.WebdriverBiDi,
#if NETCOREAPP
            EnqueueTransportMessages = false
#else
            EnqueueTransportMessages = true
#endif
        };
    }
}
