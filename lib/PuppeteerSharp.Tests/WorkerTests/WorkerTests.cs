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
            var workerCreated = new TaskCompletionSource<Worker>();
            Page.WorkerCreated += (sender, e) => workerCreated.TrySetResult(e.Worker);

            var workerObj = await Page.EvaluateExpressionHandleAsync("new Worker('data:text/javascript,1')");
            var worker = await workerCreated.Task;
            var workerThisObj = await worker.EvaluateExpressionHandleAsync("this");
            var workerDestroyed = new TaskCompletionSource<Worker>();
            Page.WorkerDestroyed += (sender, e) => workerDestroyed.TrySetResult(e.Worker);
            await Page.EvaluateFunctionAsync("workerObj => workerObj.terminate()", workerObj);
            Assert.Same(worker, await workerDestroyed.Task);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(
                () => workerThisObj.GetPropertyAsync("self"));
            Assert.Contains("Most likely the worker has been closed.", exception.Message);
        }
    }
}