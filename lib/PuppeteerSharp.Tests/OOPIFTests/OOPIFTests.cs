using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.OOPIFTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class OOPIFTests : PuppeteerPageBaseTest
    {
        public OOPIFTests(ITestOutputHelper output) : base(output)
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.Args = new[] { "--site-per-process" };
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should report oopif frames")]
        [Fact(Skip = "Ignored on puppeteer")]
        public async Task ShouldReportOopifFrames()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/dynamic-oopif.html");
            Assert.Single(Oopifs);
            Assert.Equal(2, Page.Frames.Length);
        }

        [PuppeteerTest("oopif.spec.ts", "OOPIF", "should load oopif iframes with subresources and request interception")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldLoadOopifIframesWithSubresourcesAndRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += (sender, e) => _ = e.Request.ContinueAsync();
            await Page.GoToAsync(TestConstants.ServerUrl + "/dynamic-oopif.html");
            Assert.Single(Oopifs);
        }

        private IEnumerable<Target> Oopifs => Context.Targets().Where(target => target.TargetInfo.Type == TargetType.iFrame);
    }
}
