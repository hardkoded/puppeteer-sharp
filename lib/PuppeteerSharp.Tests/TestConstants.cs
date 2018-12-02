using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Xunit;
using PuppeteerSharp.Mobile;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const int Port = 8081;
        public const int HttpsPort = Port + 1;
        public const string ServerUrl = "http://localhost:8081";
        public const string ServerIpUrl = "http://127.0.0.1:8081";
        public const string HttpsPrefix = "https://localhost:8082";
        public const string AboutBlank = "about:blank";
        public static readonly string CrossProcessHttpPrefix = "http://127.0.0.1:8081";
        public static readonly string EmptyPage = $"{ServerUrl}/empty.html";
        public static readonly string CrossProcessUrl = ServerIpUrl;
        public static readonly string ExtensionPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "simple-extension");

        public static readonly DeviceDescriptor IPhone = DeviceDescriptors.Get(DeviceDescriptorName.IPhone6);
        public static readonly DeviceDescriptor IPhone6Landscape = DeviceDescriptors.Get(DeviceDescriptorName.IPhone6Landscape);

        public static ILoggerFactory LoggerFactory { get; private set; }

        public static readonly string NestedFramesDumpResult = @"http://localhost:<PORT>/frames/nested-frames.html
    http://localhost:<PORT>/frames/two-frames.html
        http://localhost:<PORT>/frames/frame.html
        http://localhost:<PORT>/frames/frame.html
    http://localhost:<PORT>/frames/frame.html";

        public static LaunchOptions DefaultBrowserOptions() => new LaunchOptions
        {
            SlowMo = Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO")),
            Headless = Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true"),
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
                $"--disable-extensions-except={ExtensionPath}",
                $"--load-extension={ExtensionPath}"
            }
        };

        public static void SetupLogging(ITestOutputHelper output)
        {
            if (Debugger.IsAttached && LoggerFactory == null)
            {
                LoggerFactory = new LoggerFactory(new[] { new XunitLoggerProvider(output) });
            }
        }
    }
}