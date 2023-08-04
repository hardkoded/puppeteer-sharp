using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsPageErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsPageErrorTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.PageError", "should fire")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
