using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AddStyleTagTests : PuppeteerPageBaseTest
    {
        public AddStyleTagTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddStyleTagAsync(new AddTagOptions()));
            Assert.Equal("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Url = "/injectedstyle.css" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(255, 0, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Fact]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.AddStyleTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            Assert.Equal("Loading style from /nonexistfile.js failed", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Path = "Assets/injectedstyle.css" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(255, 0, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Fact]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddStyleTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedstyle.css"))
            });
            var styleHandle = await Page.QuerySelectorAsync("style");
            var styleContent = await Page.EvaluateFunctionAsync<string>("style => style.innerHTML", styleHandle);
            Assert.Contains(Path.Combine("Assets", "injectedstyle.css"), styleContent);
        }

        [Fact]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Content = "body { background-color: green; }" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(0, 128, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Fact]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddStyleTagAsync(new AddTagOptions
                {
                    Content = "body { background-color: green; }"
                }));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(
                () => Page.AddStyleTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedstyle.css"
                }));
            Assert.NotNull(exception);
        }
    }
}
