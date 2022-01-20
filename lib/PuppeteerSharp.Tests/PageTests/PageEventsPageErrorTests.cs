using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEventsPageErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsPageErrorTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.PageError", "should fire")]
        [PuppeteerFact]
        public async Task ShouldFire()
        {
            string error = null;
            void EventHandler(object sender, PageErrorEventArgs e)
            {
                error = e.Message;
                DevToolsContext.PageError -= EventHandler;
            }

            DevToolsContext.PageError += EventHandler;

            await Task.WhenAll(
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/error.html"),
                WaitEvent(DevToolsContext.Client, "Runtime.exceptionThrown")
            );

            Assert.Contains("Fancy", error);
        }
    }
}
