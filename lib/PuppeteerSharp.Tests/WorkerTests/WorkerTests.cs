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

        public async Task PageWorkers()
        {
            var pageCreatedCompletion = new TaskCompletionSource<bool>();
            Page.WorkerCreated += (sender, e) => pageCreatedCompletion.TrySetResult(true);
            await Task.WhenAll(
                    pageCreatedCompletion.Task,
                    Page.GoToAsync(TestConstants.ServerUrl + "/worker/worker.html"));
            var worker = Page.Workers()[0];
            Assert.Contains("worker.js", worker.Url);
            
            Assert.Equal("worker function result", await worker.EvaluateExpressionAsync<string>("self.workerFunction()"));

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(Page.Workers());
        }
    }
}