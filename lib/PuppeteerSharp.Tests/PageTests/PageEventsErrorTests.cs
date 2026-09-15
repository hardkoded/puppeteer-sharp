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
            // Waiter must be registered before GoToAsync — a fast chrome://crash can
            // otherwise emit Page.Error before WaitForError subscribes (CI hang abort).
            var errorTask = new TaskCompletionSource<string>();
            void EventHandler(object sender, ErrorEventArgs e)
            {
                errorTask.TrySetResult(e.Error);
                Page.Error -= EventHandler;
            }

            Page.Error += EventHandler;

            var crashUrl = TestConstants.IsChrome ? "chrome://crash" : "about:crashcontent";
            await Task.WhenAll(
                errorTask.Task,
                Page.GoToAsync(crashUrl).ContinueWith(_ => { }));

            Assert.That(await errorTask.Task, Is.EqualTo("Page crashed!"));
        }
    }
}
