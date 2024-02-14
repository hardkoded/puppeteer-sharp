using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.DefaultBrowserContextTests
{
    public class BrowserTargetEventsTests : PuppeteerBaseTest
    {
        public BrowserTargetEventsTests() : base()
        {
        }

        [PuppeteerTest("defaultbrowsercontext.spec.ts", "Browser target events", "should work")]
        public async Task ShouldWork()
        {
            using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());

            var events = new List<string>();
            browser.TargetCreated += (_, _) => events.Add("CREATED");
            browser.TargetChanged += (_, _) => events.Add("CHANGED");
            browser.TargetDestroyed += (_, _) => events.Add("DESTROYED");
            var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();

            Assert.AreEqual(new[] { "CREATED", "CHANGED", "DESTROYED" }, events);
        }
    }
}
