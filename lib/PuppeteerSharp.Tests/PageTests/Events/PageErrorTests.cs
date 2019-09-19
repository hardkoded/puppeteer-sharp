using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageErrorTests : PuppeteerPageBaseTest
    {
        public PageErrorTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFire()
        {
            string error = null;
            void EventHandler(object sender, PageErrorEventArgs e)
            {
                error = e.Message;
                Page.PageError -= EventHandler;
            }

            Page.PageError += EventHandler;

            await Task.WhenAll(
                Page.GoToAsync(TestConstants.ServerUrl + "/error.html"),
                WaitEvent(Page.Client, "Runtime.exceptionThrown")
            );

            Assert.Contains("Fancy", error);
        }
    }
}
