using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageEventsPopupTests : PuppeteerPageBaseTest
    {
        public PageEventsPopupTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work")]
        public async Task ShouldWork()
        {
            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.True(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work with noopener")]
        public async Task ShouldWorkWithNoopener()
        {
            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank', null, 'noopener')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work with clicking target=_blank and without rel=opener")]
        public async Task ShouldWorkWithClickingTargetBlankWithoutRelOpener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work with clicking target=_blank and with rel=opener")]
        public async Task ShouldWorkWithFakeClickingTargetBlankAndRelOpener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=opener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.QuerySelectorAsync("a").EvaluateFunctionAsync("a => a.click()"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.True(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work with fake-clicking target=_blank and rel=noopener")]
        public async Task ShouldWorkWithFakeClickingTargetBlankAndRelNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.QuerySelectorAsync("a").EvaluateFunctionAsync("a => a.click()"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.Events.Popup", "should work with clicking target=_blank and rel=noopener")]
        public async Task ShouldWorkWithClickingTargetBlankAndRelNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<IPage>();
            Page.Popup += (_, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }
    }
}
