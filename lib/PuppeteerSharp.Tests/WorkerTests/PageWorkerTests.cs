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

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "Page.workers")]
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
            StringAssert.Contains("worker.js", worker.Url);
            Assert.AreEqual("worker function result", await worker.EvaluateExpressionAsync<string>("self.workerFunction()"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            await workerDestroyedTcs.Task.WithTimeout();
            Assert.IsEmpty(Page.Workers);
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "should emit created and destroyed events")]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.AreSame(worker, await workerDestroyedTcs.Task);
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "should report console logs")]
        public async Task ShouldReportConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");

            var log = await consoleTcs.Task;
            Assert.AreEqual("1", log.Text);
            Assert.AreEqual(new ConsoleMessageLocation
            {
                URL = "",
                LineNumber = 0,
                ColumnNumber = 8
            }, log.Location);
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "should have JSHandles for console logs")]
        public async Task ShouldHaveJSHandlesForConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (_, e) =>
            {
                consoleTcs.TrySetResult(e.Message);
            };
            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1, 2, 3, this)`)");
            var log = await consoleTcs.Task;
            Assert.AreEqual("1 2 3 JSHandle@object", log.Text);
            Assert.AreEqual(4, log.Args.Count);
            var json = await (await log.Args[3].GetPropertyAsync("origin")).JsonValueAsync<object>();
            Assert.AreEqual("null", json);
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "should have an execution context")]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.AreEqual(2, await worker.EvaluateExpressionAsync<int>("1+1"));
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "should report errors")]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>();
            Page.PageError += (_, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            StringAssert.Contains("this is my error", errorLog);
        }

        [Test, Retry(2), PuppeteerTest("worker.spec", "Workers", "scan be closed")]
        public async Task CanBeClosed()
        {
            var workerCreatedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            await workerCreatedTcs.Task;
            var worker = Page.Workers[0];
            var workerClosedTcs = new TaskCompletionSource<WebWorker>();
            Page.WorkerDestroyed += (_, e) => workerClosedTcs.TrySetResult(e.Worker);

            StringAssert.Contains("worker.js", worker.Url);
            await worker.CloseAsync();
            Assert.AreSame(worker, await workerClosedTcs.Task);
        }
    }
}
