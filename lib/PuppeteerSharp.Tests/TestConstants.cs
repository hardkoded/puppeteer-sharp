using System;
using System.Collections.Generic;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const int HttpsPort = 8908;
        public const string ServerUrl = "http://localhost:8907";
        public const string HttpsPrefix = "https://localhost:8908";
        public const int ChromiumRevision = 526987;
        public static readonly string EmptyPage = $"{ServerUrl}/empty.html";

        public static readonly Dictionary<string, object> DefaultBrowserOptions = new Dictionary<string, object>()
        {
            { "slowMo", Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO") ?? "0") },
            { "headless", Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true") },
            { "args", new[] { "--no-sandbox" }},
            { "timeout", 0},
            { "keepAliveInterval", 120}
        };


    }
}
