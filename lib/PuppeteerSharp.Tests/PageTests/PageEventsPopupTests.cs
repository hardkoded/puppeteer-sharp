using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEventsPopupTests : PuppeteerPageBaseTest
    {
        public PageEventsPopupTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.True(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work with noopener")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithNoopener()
        {
            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank', null, 'noopener')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work with clicking target=_blank and without rel=opener")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithClickingTargetBlankWithoutRelOpener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work with clicking target=_blank and with rel=opener")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithFakeClickingTargetBlankAndReNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.QuerySelectorAsync("a").EvaluateFunctionAsync("a => a.click()"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work with fake-clicking target=_blank and rel=noopener")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithFakeClickingTargetBlankAndRelNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.QuerySelectorAsync("a").EvaluateFunctionAsync("a => a.click()"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [PuppeteerTest("page.spec.ts", "Page.Events.Popup", "should work with clicking target=_blank and rel=noopener")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithClickingTargetBlankAndRelNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }
    }
}
