using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsErrorTests : PuppeteerPageBaseTest
    {
        public PageEventsErrorTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.error", "should throw when page crashes")]
        public async Task ShouldThrowWhenPageCrashes()
        {
            string error = null;
            Page.Error += (_, e) => error = e.Error;
            var crashUrl = TestConstants.IsChrome ? "chrome://crash" : "about:crashcontent";
            var gotoTask = Page.GoToAsync(crashUrl);

            await WaitForError();
            Assert.That(error, Is.EqualTo("Page crashed!"));
        }
    }
}
