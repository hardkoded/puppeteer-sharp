using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEventsCloseTests : PuppeteerPageBaseTest
    {
        public PageEventsCloseTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Close", "should work with window.close")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithWindowClose()
        {
            var newPageTaskSource = new TaskCompletionSource<Page>();
            Context.TargetCreated += async (_, e) => newPageTaskSource.TrySetResult(await e.Target.PageAsync());

            await Page.EvaluateExpressionAsync("window['newPage'] = window.open('about:blank');");
            var newPage = await newPageTaskSource.Task;

            var closeTaskSource = new TaskCompletionSource<bool>();
            newPage.Close += (_, _) => closeTaskSource.SetResult(true);
            await Page.EvaluateExpressionAsync("window['newPage'].close();");
            await closeTaskSource.Task;
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Close", "should work with page.close")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithPageClose()
        {
            var newPage = await Context.NewPageAsync();
            var closeTaskSource = new TaskCompletionSource<bool>();
            newPage.Close += (_, _) => closeTaskSource.SetResult(true);
            await newPage.CloseAsync();
            await closeTaskSource.Task;
        }
    }
}
