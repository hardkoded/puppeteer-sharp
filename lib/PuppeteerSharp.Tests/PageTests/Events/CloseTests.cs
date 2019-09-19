using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CloseTests : PuppeteerPageBaseTest
    {
        public CloseTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWorkWithWindowClose()
        {
            var newPageTaskSource = new TaskCompletionSource<Page>();
            Context.TargetCreated += async (sender, e) => newPageTaskSource.TrySetResult(await e.Target.PageAsync());

            await Page.EvaluateExpressionAsync("window['newPage'] = window.open('about:blank');");
            var newPage = await newPageTaskSource.Task;

            var closeTaskSource = new TaskCompletionSource<bool>();
            newPage.Close += (sender, e) => closeTaskSource.SetResult(true);
            await Page.EvaluateExpressionAsync("window['newPage'].close();");
            await closeTaskSource.Task;
        }

        [Fact]
        public async Task ShouldWorkWithPageClose()
        {
            var newPage = await Context.NewPageAsync();
            var closeTaskSource = new TaskCompletionSource<bool>();
            newPage.Close += (sender, e) => closeTaskSource.SetResult(true);
            await newPage.CloseAsync();
            await closeTaskSource.Task;
        }
    }
}