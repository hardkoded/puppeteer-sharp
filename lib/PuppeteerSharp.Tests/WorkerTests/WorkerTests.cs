using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.WorkerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class WorkerTests : PuppeteerPageBaseTest
    {
        public WorkerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task PageWorkers()
        {
            var pageCreatedCompletion = new TaskCompletionSource<bool>();
            Page.WorkerCreated += (sender, e) => pageCreatedCompletion.TrySetResult(true);
            await Task.WhenAll(
                    pageCreatedCompletion.Task,
                    Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html"));
            var worker = Page.Workers[0];
            Assert.Contains("worker.js", worker.Url);

            Assert.Equal("worker function result", await worker.EvaluateExpressionAsync<string>("self.workerFunction()"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(Page.Workers);
        }

        [Fact]
        public async Task ShouldEmitCreatedAndDestroyedEvents()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerCreated += (sender, e) => workerCreatedTcs.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateFunctionHandleAsync("() => new Worker('data:text/javascript,1')");
            var worker = await workerCreatedTcs.Task;
            var workerDestroyedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerDestroyed += (sender, e) => workerDestroyedTcs.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.Same(worker, await workerDestroyedTcs.Task);
        }

        [Fact]
        public async Task ShouldReportConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (sender, e) => consoleTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");

            var log = await consoleTcs.Task;
            Assert.Equal("1", log.Text);
        }

        [Fact]
        public async Task ShouldHaveJSHandlesForConsoleLogs()
        {
            var consoleTcs = new TaskCompletionSource<ConsoleMessage>();
            Page.Console += (sender, e) =>
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

        [Fact]
        public async Task ShouldHaveAnExecutionContext()
        {
            var workerCreatedTcs = new TaskCompletionSource<Worker>();
            Page.WorkerCreated += (sender, e) => workerCreatedTcs.TrySetResult(e.Worker);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript,console.log(1)`)");
            var worker = await workerCreatedTcs.Task;
            Assert.Equal(2, await worker.EvaluateExpressionAsync<int>("1+1"));
        }

        [Fact]
        public async Task ShouldReportErrors()
        {
            var errorTcs = new TaskCompletionSource<string>();
            Page.PageError += (sender, e) => errorTcs.TrySetResult(e.Message);

            await Page.EvaluateFunctionAsync("() => new Worker(`data:text/javascript, throw new Error('this is my error');`)");
            var errorLog = await errorTcs.Task;
            Assert.Contains("this is my error", errorLog);
        }
    }
}