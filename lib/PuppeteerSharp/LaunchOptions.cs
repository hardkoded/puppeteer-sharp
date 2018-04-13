using System;
using System.Collections.Generic;

namespace PuppeteerSharp
{
    public class LaunchOptions : BrowserOptions
    {        
        public bool Headless { get; set; } = true;

        public string ExecutablePath { get; set; }

        public int SlowMo { get; set; }

        public string[] Args { get; set; } = Array.Empty<string>();
        
        public int Timeout { get; set; } = 30_000;

        public bool DumpIO { get; set; }

        public string UserDataDir { get; set; }

        public IDictionary<string, string> Env { get; } = new Dictionary<string, string>();

        public bool Devtools { get; set; }

        public int KeepAliveInterval { get; set; } = 30;

        public bool LogProcess { get; set; }
    }
}
