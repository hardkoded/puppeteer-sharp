using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LauncherTests
{
    public class BrowserTargetEventsTests : PuppeteerBrowserBaseTest
    {
        public BrowserTargetEventsTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("launcher.spec", "Launcher specs Browser target events", "should work")]
        public async Task ShouldWork()
        {
            await using var browser = await Puppeteer.LaunchAsync(TestConstants.DefaultBrowserOptions());
            var events = new List<string>();

            Browser.TargetCreated += (_, _) => events.Add("CREATED");
            Browser.TargetChanged += (_, _) => events.Add("CHANGED");
            Browser.TargetDestroyed += (_, _) => events.Add("DESTROYED");
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            // Wait for half a second to ensure that all events have been processed
            await Task.Delay(500);
            Assert.AreEqual(new[] { "CREATED", "CHANGED", "DESTROYED" }, events);
        }
    }
}
