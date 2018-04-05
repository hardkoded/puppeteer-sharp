using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EventsErrorTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            Page.Error += (sender, args) => error = args.Error;
            var gotoTask = Page.GoToAsync("chrome://crash");

            await WaitForError();
            Assert.Equal("Page crashed!", error);
        }
    }
}
