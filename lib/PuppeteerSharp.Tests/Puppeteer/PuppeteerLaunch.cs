using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Xunit;
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.Puppeteer
{
    public class PuppeteerLaunch
    {
        private Dictionary<string, object> _defaultBrowserOptions = new Dictionary<string, object>()
        {
            { "executablePath", Environment.GetEnvironmentVariable("CHROME") },
            { "slowMo", Convert.ToInt32(Environment.GetEnvironmentVariable("SLOW_MO") ?? "0") },
            { "headless", Convert.ToBoolean(Environment.GetEnvironmentVariable("HEADLESS") ?? "true") },
            { "args", new[] { "--no-sandbox" }}
        };

        [Fact]
        public async Task should_support_ignoreHTTPSErrors_option()
        {
            var options = _defaultBrowserOptions.Clone();
            options.Add("ignoreHTTPSErrors", true);
            /*
            var browser = await puppeteer.launch(options);
            const page = await browser.newPage();
            let error = null;
            const response = await page.goto(HTTPS_PREFIX + '/empty.html').catch (e => error = e);
            expect(error).toBe(null);
            expect(response.ok).toBe(true);
            browser.close()
            */
        }
    }
}
