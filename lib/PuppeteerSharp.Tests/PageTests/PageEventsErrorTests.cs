using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsErrorTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Error", "should throw when page crashes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            Page.Error += (_, e) => error = e.Error;
            var gotoTask = Page.GoToAsync("chrome://crash");

            await WaitForError();
            Assert.Equal("Page crashed!", error);
        }
    }
}
