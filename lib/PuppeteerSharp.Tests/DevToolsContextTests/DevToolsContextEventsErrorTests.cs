using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEventsErrorTests : PuppeteerPageBaseTest
    {
        public DevToolsContextEventsErrorTests(ITestOutputHelper output) : base(output)
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

        [PuppeteerTest("page.spec.ts", "Page.Events.Error", "should throw when page crashes")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            DevToolsContext.Error += (_, e) => error = e.Error;
            var gotoTask = DevToolsContext.GoToAsync("chrome://crash");

            await WaitForError();
            Assert.Equal("Page crashed!", error);
        }
    }
}
