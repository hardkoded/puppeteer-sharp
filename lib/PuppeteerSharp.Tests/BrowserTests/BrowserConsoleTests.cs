using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class BrowserConsoleTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"console\")", "should report console messages from pages")]
        public async Task ShouldReportConsoleMessagesFromPages()
        {
            var messageTask = new TaskCompletionSource<ConsoleMessage>();
            void Handler(object _, ConsoleEventArgs e)
            {
                if (e.Message.Text == "from-page")
                {
                    messageTask.TrySetResult(e.Message);
                }
            }

            Browser.Console += Handler;
            try
            {
                await Task.WhenAll(
                    messageTask.Task,
                    Page.EvaluateFunctionAsync("() => console.log('from-page')"));

                Assert.That(messageTask.Task.Result.Type, Is.EqualTo(ConsoleType.Log));
            }
            finally
            {
                Browser.Console -= Handler;
            }
        }

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"console\")", "should report console messages from service workers")]
        public async Task ShouldReportConsoleMessagesFromServiceWorkers()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/empty/sw.html");
            var target = await Context.WaitForTargetAsync(t => t.Type == TargetType.ServiceWorker, new WaitForOptions(3_000));
            var worker = await target.WorkerAsync();

            var messageTask = new TaskCompletionSource<ConsoleMessage>();
            void Handler(object _, ConsoleEventArgs e)
            {
                if (e.Message.Text == "from-service-worker")
                {
                    messageTask.TrySetResult(e.Message);
                }
            }

            Browser.Console += Handler;
            try
            {
                await Task.WhenAll(
                    messageTask.Task,
                    worker.EvaluateFunctionAsync("() => console.log('from-service-worker')"));

                Assert.That(messageTask.Task.Result.Type, Is.EqualTo(ConsoleType.Log));
            }
            finally
            {
                Browser.Console -= Handler;
            }
        }
    }
}
