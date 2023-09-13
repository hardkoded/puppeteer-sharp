using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.DefaultBrowserContextTests
{
    public class BrowserTargetEventsTests : PuppeteerBaseTest
    {
        public BrowserTargetEventsTests(): base()
        {
        }

        [PuppeteerTest("defaultbrowsercontext.spec.ts", "Browser target events", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());

            // .NET has a race here where we get the target created of the default page.
            // We will wait for the new page before moving on.
            await browser.PagesAsync();
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
