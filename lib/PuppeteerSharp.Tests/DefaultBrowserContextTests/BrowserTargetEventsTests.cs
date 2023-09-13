using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;
using NUnit.Framework.Internal.Execution;

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
            var events = new List<string>();
            browser.TargetCreated += (_, t) => events.Add($"CREATED {t.Target.GetHashCode()}");
            browser.TargetChanged += (_, t) => events.Add($"CHANGED {t.Target.GetHashCode()}");
            browser.TargetDestroyed += (_, t) => events.Add($"DESTROYED {t.Target.GetHashCode()}");
            var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();

            // TODO: Review why these might come in different order
            Assert.AreEqual(3, events.Count, $"Events: {string.Join(", ", events)}");
            Assert.That(events, Does.Contain("CREATED"));
            Assert.That(events, Does.Contain("CHANGED"));
            Assert.That(events, Does.Contain("DESTROYED"));
        }
    }
}
