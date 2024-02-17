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

        [Test, PuppeteerTimeout, PuppeteerTest("page.spec", "Page Page.Events.Error", "should throw when page crashes")]
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
