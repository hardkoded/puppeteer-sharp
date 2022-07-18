using System;
using System.Collections.Generic;
using System.IO;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Mobile;

namespace PuppeteerSharp.Tests
{
    public static class TestConstants
    {
        public const string TestFixtureCollectionName = "PuppeteerLoaderFixture collection";
        public const int DebuggerAttachedTestTimeout = 300_000;
        public const int DefaultTestTimeout = 30_000;
        public const int DefaultPuppeteerTimeout = 10_000;
        public const int Port = 8088;
        public const int HttpsPort = Port + 1;
        public const string ServerUrl = "http://localhost:8088";
        public const string ServerIpUrl = "http://127.0.0.1:8088";
        public const string HttpsPrefix = "https://localhost:8089";
        public const string AboutBlank = "about:blank";
        public static readonly string CrossProcessHttpPrefix = "http://127.0.0.1:8088";
        public static readonly string EmptyPage = $"{ServerUrl}/empty.html";
        public static readonly string CrossProcessUrl = ServerIpUrl;

        public static readonly DeviceDescriptor IPhone = Emulation.Devices[DeviceDescriptorName.IPhone6];
        public static readonly DeviceDescriptor IPhone6Landscape = Emulation.Devices[DeviceDescriptorName.IPhone6Landscape];

        public static string FileToUpload => Path.Combine(AppContext.BaseDirectory, "Assets", "file-to-upload.txt");

        public static readonly IEnumerable<string> NestedFramesDumpResult = new List<string>()
        {
            "http://localhost:<PORT>/frames/nested-frames.html",
            "    http://localhost:<PORT>/frames/two-frames.html (2frames)",
            "        http://localhost:<PORT>/frames/frame.html (uno)",
            "        http://localhost:<PORT>/frames/frame.html (dos)",
            "    http://localhost:<PORT>/frames/frame.html (aframe)"
        };
    }
}
