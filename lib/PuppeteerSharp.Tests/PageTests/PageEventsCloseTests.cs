using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsCloseTests : PuppeteerPageBaseTest
    {
        public PageEventsCloseTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Close", "should work with window.close")]
        public async Task ShouldWorkWithWindowClose()
        {
            var newPageTaskSource = new TaskCompletionSource<IPage>();
            Context.TargetCreated += async (_, e) => newPageTaskSource.TrySetResult(await e.Target.PageAsync());

            await Page.EvaluateExpressionAsync("window['newPage'] = window.open('about:blank');");
            var newPage = await newPageTaskSource.Task.WithTimeout();

            var closeTaskSource = new TaskCompletionSource<bool>();
            newPage.Close += (_, _) => closeTaskSource.SetResult(true);
            await Page.EvaluateExpressionAsync("window['newPage'].close();");
            await closeTaskSource.Task.WithTimeout();
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Close", "should work with page.close")]
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
