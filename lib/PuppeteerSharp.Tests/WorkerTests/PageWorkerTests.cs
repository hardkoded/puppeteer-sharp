using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.WorkerTests
{
    public class PageWorkerTests : PuppeteerPageBaseTest
    {
        public PageWorkerTests(): base()
        {
        }

        [PuppeteerTest("worker.spec.ts", "Workers", "Page.workers")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.Contains("worker.js", worker.Url);

            Assert.AreEqual("worker function result", await worker.EvaluateExpressionAsync<string>("self.workerFunction()"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            await workerDestroyedTcs.Task.WithTimeout();
            Assert.IsEmpty(Page.Workers);
        }

        [PuppeteerTest("worker.spec.ts", "Workers", "should emit created and destroyed events")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerDestroyed += (_, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.Same(worker, await workerDestroyedTcs.Task);
        }

        [PuppeteerTest("worker.spec.ts", "Workers", "should report console logs")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("worker.spec.ts", "Workers", "should have JSHandles for console logs")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("worker.spec.ts", "Workers", "should have an execution context")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.AreEqual(2, await worker.EvaluateExpressionAsync<int>("1+1"));
        }

        [PuppeteerTest("worker.spec.ts", "Workers", "should report errors")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>();
            Page.PageError += (_, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            Assert.Contains("this is my error", errorLog);
        }
    }
}
