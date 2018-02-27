using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class LaunchOptions
    {
        public bool AppMode { get; set; }

        public bool IgnoreHTTPSErrors { get; set; }

        public bool IgnoreDefaultArgs { get; set; }

        public bool Headless { get; set; } = true;

        public string ExecutablePath { get; set; }

        public int SlowMo { get; set; }

        public string[] Args { get; set; } = Array.Empty<string>();

        public bool HandleSIGINT { get; set; } = true;

        public bool HandleSIGTERM { get; set; } = true;

        public bool HandleSIGHUP { get; set; } = true;

        public int Timeout { get; set; } = 30_000;

        public bool Dumpio { get; set; }

        public string UserDataDir { get; set; }

        public IDictionary<string, string> Env { get; } = new Dictionary<string, string>();

        public bool Devtools { get; set; }

        public int KeepAliveInterval { get; set; } = 30;
    }
}
