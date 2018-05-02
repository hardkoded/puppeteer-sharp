
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class PageErrorTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldFire()
        {
            string error = null;
            void EventHandler(object sender, PageErrorEventArgs e)
            {
                error = e.Error.Exception.Description;
                Page.PageError -= EventHandler;
            }

            Page.PageError += EventHandler;

            await Task.WhenAll(
                Page.GoToAsync(TestConstants.ServerUrl + "/error.html"),
                WaitForEvents(Page.Client, "Runtime.exceptionThrown")
            );

            Assert.Contains("Fancy", error);
        }
    }
}
