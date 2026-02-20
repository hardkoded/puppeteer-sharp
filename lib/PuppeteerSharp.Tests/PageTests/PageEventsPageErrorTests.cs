using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsPageErrorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.Events.PageError", "should fire")]
        public async Task ShouldFire()
        {
            var errorTask = new TaskCompletionSource<string>();
            void EventHandler(object sender, PageErrorEventArgs e)
            {
                if (e.Message.Contains("Fancy"))
                {
                    errorTask.TrySetResult(e.Message);
                }
                Page.PageError -= EventHandler;
            }

            Page.PageError += EventHandler;

            await Task.WhenAll(
                errorTask.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/error.html")
            );

            var error = await errorTask.Task;
            Assert.That(error, Does.Contain("Fancy"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.Events.PageError", "should fire for all value types")]
        public async Task ShouldFireForAllValueTypes()
        {
            var errorTask = new TaskCompletionSource<PageErrorEventArgs>();
            void EventHandler(object sender, PageErrorEventArgs e)
            {
                errorTask.TrySetResult(e);
                Page.PageError -= EventHandler;
            }

            Page.PageError += EventHandler;

            await Task.WhenAll(
                errorTask.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/error-primitive.html")
            );

            var error = await errorTask.Task;
            Assert.That(error.Error, Is.Null);
        }
    }
}
