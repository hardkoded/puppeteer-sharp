using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.WorkerTests
{
    public class PageWorkerTests : PuppeteerPageBaseTest
    {
        public PageWorkerTests() : base()
        {
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "Page.workers")]
        [Ignore("TODO: Fix me. Too flaky")]
        public async Task PageWorkers()
        {
            var workerCreatedTcs = new TaskCompletionSource<bool>();
            var workerDestroyedTcs = new TaskCompletionSource<bool>();

            Page.WorkerCreated += (_, _) => workerCreatedTcs.TrySetResult(true);
            Page.WorkerDestroyed += (_, _) => workerDestroyedTcs.TrySetResult(true);

            await Task.WhenAll(
                workerCreatedTcs.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html"));
            var worker = Page.Workers[0];
            Assert.That(worker.Url, Does.Contain("worker.js"));
            Assert.That(await worker.EvaluateExpressionAsync<string>("globalThis.workerFunction()"), Is.EqualTo("worker function result"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            await workerDestroyedTcs.Task.WithTimeout();
            Assert.That(Page.Workers, Is.Empty);
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should emit created and destroyed events")]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.That(await workerDestroyedTcs.Task, Is.SameAs(worker));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should report console logs")]
        public async Task ShouldReportConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");

            var log = await consoleTcs.Task;
            Assert.That(log.Text, Is.EqualTo("1"));
            Assert.That(log.Location, Is.EqualTo(new ConsoleMessageLocation
            {
                URL = "",
                LineNumber = 0,
                ColumnNumber = 8
            }));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should have JSHandles for console logs")]
        public async Task ShouldHaveJSHandlesForConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) =>
            {
                consoleTcs.TrySetResult(e.Message);
            };
            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1, 2, 3, this)`)");
            var log = await consoleTcs.Task;

            Assert.That(log.Text, Is.EqualTo("1 2 3 JSHandle@object"));
            Assert.That(log.Args.Count, Is.EqualTo(4));
            var json = await (await log.Args[3].GetPropertyAsync("origin")).JsonValueAsync<string>();
            Assert.That(json, Is.EqualTo("null"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should have an execution context")]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.That(await worker.EvaluateExpressionAsync<int>("1+1"), Is.EqualTo(2));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "should report errors")]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>();
            Page.PageError += (_, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            Assert.That(errorLog, Does.Contain("this is my error"));
        }

        [Test, PuppeteerTest("worker.spec", "Workers", "scan be closed")]
        public async Task CanBeClosed()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            await workerCreatedTcs.Task;
            var worker = Page.Workers[0];
            var workerClosedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerClosedTcs.TrySetResult(e.Worker);

            Assert.That(worker.Url, Does.Contain("worker.js"));
            await worker.CloseAsync();
            Assert.That(await workerClosedTcs.Task, Is.SameAs(worker));
        }
    }
}
