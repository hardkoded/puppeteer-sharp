using System;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const int HttpsPort = 8908;
        public const string ServerUrl = "http://localhost:8907";
        public const string ServerIpUrl = "http://127.0.0.1:8907";
        public const string HttpsPrefix = "https://localhost:8908";
        public const int ChromiumRevision = Downloader.DefaultRevision;
        public const string AboutBlank = "about:blank";
        public static readonly string CrossProcessHttpPrefix = "http://127.0.0.1:8907";
        public static readonly string EmptyPage = $"{ServerUrl}/empty.html";
        public static readonly string CrossProcessAuthenticateUrl = $"{ServerIpUrl}/authenticationtest/testuser";
        public static readonly string AuthenticateUrl = $"{ServerUrl}/authenticationtest/testuser";

        public static LaunchOptions DefaultBrowserOptions() => new LaunchOptions
        {
            SlowMo = Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO")),
            Headless = Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true"),
            Args = new[] { "--no-sandbox" },
            Timeout = 0,
            KeepAliveInterval = 120,
            LogProcess = true
        };
    }
}
