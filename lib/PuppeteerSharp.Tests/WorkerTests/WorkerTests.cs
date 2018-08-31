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
            await Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html");
            await Page.WaitForFunctionAsync("() => !!worker");
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
    }
}