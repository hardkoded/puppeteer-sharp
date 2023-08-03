using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.DefaultBrowserContextTests
{
    public class BrowserTargetEventsTests : PuppeteerBrowserBaseTest
    {
        public BrowserTargetEventsTests(): base()
        {
        }

        [PuppeteerTest("defaultbrowsercontext.spec.ts", "Browser target events", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
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
