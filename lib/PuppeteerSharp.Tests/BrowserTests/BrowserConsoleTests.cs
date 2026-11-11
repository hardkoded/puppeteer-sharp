using System.Collections.Generic;
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

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.on(\"console\")", "should report console messages from dedicated workers")]
        public async Task ShouldReportConsoleMessagesFromDedicatedWorkers()
        {
            const string workerUrl = "data:text/javascript,console.log('from-worker');console.log('done')";
            var pageMessages = new List<ConsoleMessage>();
            var contextMessages = new List<ConsoleMessage>();
            var browserMessages = new List<ConsoleMessage>();
            var workerCreated = new TaskCompletionSource<WebWorker>();
            var done = new TaskCompletionSource<ConsoleMessage>();

            void OnWorkerCreated(object _, WorkerEventArgs e) => workerCreated.TrySetResult(e.Worker);

            void OnPageConsole(object _, ConsoleEventArgs e)
            {
                if (e.Message.Text == "from-worker")
                {
                    pageMessages.Add(e.Message);
                }
            }

            void OnContextConsole(object _, ConsoleEventArgs e)
            {
                if (e.Message.Text == "from-worker")
                {
                    contextMessages.Add(e.Message);
                }
            }

            void OnBrowserConsole(object _, ConsoleEventArgs e)
            {
                if (e.Message.Text == "from-worker")
                {
                    browserMessages.Add(e.Message);
                }
                else if (e.Message.Text == "done")
                {
                    done.TrySetResult(e.Message);
                }
            }

            Page.WorkerCreated += OnWorkerCreated;
            Page.Console += OnPageConsole;
            Context.Console += OnContextConsole;
            Browser.Console += OnBrowserConsole;
            try
            {
                // "done" is logged after "from-worker", so once it arrives every delivery of "from-worker" has happened.
                await Task.WhenAll(
                    workerCreated.Task,
                    done.Task,
                    Page.EvaluateFunctionAsync("url => new Worker(url)", workerUrl));

                Assert.That(pageMessages, Has.Count.EqualTo(1));
                Assert.That(contextMessages, Has.Count.EqualTo(1));
                Assert.That(browserMessages, Has.Count.EqualTo(1));
                Assert.That(pageMessages[0], Is.SameAs(browserMessages[0]));
                Assert.That(contextMessages[0], Is.SameAs(browserMessages[0]));
            }
            finally
            {
                Page.WorkerCreated -= OnWorkerCreated;
                Page.Console -= OnPageConsole;
                Context.Console -= OnContextConsole;
                Browser.Console -= OnBrowserConsole;
            }
        }
    }
}
