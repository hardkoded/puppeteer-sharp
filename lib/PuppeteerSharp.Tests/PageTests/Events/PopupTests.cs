using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PopupTests : PuppeteerPageBaseTest
    {
        public PopupTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (sender, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.True(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Fact]
        public async Task ShouldWorkWithNoopener()
        {
            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (sender, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.EvaluateExpressionAsync("window.open('about:blank', null, 'noopener')"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Fact]
        public async Task ShouldWorkWithClickingTargetBlank()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (sender, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.True(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Fact]
        public async Task ShouldWorkWithFakeClickingTargetBlankAndReNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (sender, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.QuerySelectorAsync("a").EvaluateFunctionAsync("a => a.click()"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }

        [Fact]
        public async Task ShouldWorkWithClickingTargetBlankAndRelNoopener()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetContentAsync("<a target=_blank rel=noopener href='/one-style.html'>yo</a>");

            var popupTaskSource = new TaskCompletionSource<Page>();
            Page.Popup += (sender, e) => popupTaskSource.TrySetResult(e.PopupPage);

            await Task.WhenAll(
                popupTaskSource.Task,
                Page.ClickAsync("a"));

            Assert.False(await Page.EvaluateExpressionAsync<bool>("!!window.opener"));
            Assert.False(await popupTaskSource.Task.Result.EvaluateExpressionAsync<bool>("!!window.opener"));
        }
    }
}