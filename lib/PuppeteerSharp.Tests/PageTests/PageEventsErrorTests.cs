using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
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
