using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PuppeteerSharp
{
    public class Puppeteer
    {
        public Puppeteer()
        {
        }

        public static async void Launch(Dictionary<string, object> options) {
            Launcher.Launch(options);
        }
    }
}
