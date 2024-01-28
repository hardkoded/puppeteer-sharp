using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsErrorTests() : base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Error", "should throw when page crashes")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            Page.Error += (_, e) => error = e.Error;
            var gotoTask = Page.GoToAsync("chrome://crash");

            await WaitForError();
            Assert.AreEqual("Page crashed!", error);
        }
    }
}
