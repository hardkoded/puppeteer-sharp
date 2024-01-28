using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserTargetEventsTests : PuppeteerBrowserBaseTest
    {
        public BrowserTargetEventsTests() : base()
        {
        }

        [PuppeteerTest("launcher.spec.ts", "Browser target events", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var events = new List<string>();

            Browser.TargetCreated += (_, _) => events.Add("CREATED");
            Browser.TargetChanged += (_, _) => events.Add("CHANGED");
            Browser.TargetDestroyed += (_, _) => events.Add("DESTROYED");
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            Assert.AreEqual(new[] { "CREATED", "CHANGED", "DESTROYED" }, events);
        }
    }
}
