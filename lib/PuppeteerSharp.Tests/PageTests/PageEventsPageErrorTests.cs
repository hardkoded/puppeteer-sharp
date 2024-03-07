using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsPageErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsPageErrorTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.PageError", "should fire")]
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

            StringAssert.Contains("Fancy", error);
        }
    }
}
