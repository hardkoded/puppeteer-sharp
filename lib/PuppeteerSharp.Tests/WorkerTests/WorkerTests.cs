using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.WorkerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class WorkerTests : PuppeteerPageBaseTest
    {
        public WorkerTests(ITestOutputHelper output) : base(output)
        {
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageWorkers()
        {
            var workerCreatedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            var workerDestroyedTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

            Page.WorkerCreated += (_, _) => workerCreatedTcs.TrySetResult(true);
            Page.WorkerDestroyed += (_, _) => workerDestroyedTcs.TrySetResult(true);

            await Task.WhenAll(
                workerCreatedTcs.Task,
                Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html"));
            var worker = Page.Workers[0];
            Assert.Contains("worker.js", worker.Url);

            Assert.Equal("worker function result", await worker.EvaluateExpressionAsync<string>("self.workerFunction()"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            await workerDestroyedTcs.Task.WithTimeout();
            Assert.Empty(Page.Workers);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<Worker>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.WorkerDestroyed += (_, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.Same(worker, await workerDestroyedTcs.Task);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.Console += (_, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");

            var log = await consoleTcs.Task;
            Assert.Equal("1", log.Text);
            Assert.Equal(new ConsoleMessageLocation
            {
                URL = "",
                LineNumber = 0,
                ColumnNumber = 8
            }, log.Location);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldHaveJSHandlesForConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.Console += (_, e) =>
            {
                consoleTcs.TrySetResult(e.Message);
            };
            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1, 2, 3, this)`)");
            var log = await consoleTcs.Task;
            Assert.Equal("1 2 3 JSHandle@object", log.Text);
            Assert.Equal(4, log.Args.Count);
            var json = await (await log.Args[3].GetPropertyAsync("origin")).JsonValueAsync<object>();
            Assert.Equal("null", json);
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.WorkerCreated += (_, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.Equal(2, await worker.EvaluateExpressionAsync<int>("1+1"));
        }

        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            Page.PageError += (_, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            Assert.Contains("this is my error", errorLog);
        }
    }
}
