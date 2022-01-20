using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using Xunit;
using Xunit.Abstractions;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AddStyleTagTests : PuppeteerPageBaseTest
    {
        public AddStyleTagTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should throw an error if no options are provided")]
        [PuppeteerFact]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(()
                => DevToolsContext.AddStyleTagAsync(new AddTagOptions()));
            Assert.Equal("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should work with a url")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAUrl()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await DevToolsContext.AddStyleTagAsync(new AddTagOptions { Url = "/injectedstyle.css" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(255, 0, 0)", await DevToolsContext.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should throw an error if loading from url fail")]
        [PuppeteerFact]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => DevToolsContext.AddStyleTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            Assert.Equal("Loading style from /nonexistfile.js failed", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should work with a path")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAPath()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await DevToolsContext.AddStyleTagAsync(new AddTagOptions { Path = "Assets/injectedstyle.css" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(255, 0, 0)", await DevToolsContext.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should include sourcemap when path is provided")]
        [PuppeteerFact]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddStyleTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedstyle.css"))
            });
            var styleHandle = await DevToolsContext.QuerySelectorAsync("style");
            var styleContent = await DevToolsContext.EvaluateFunctionAsync<string>("style => style.innerHTML", styleHandle);
            Assert.Contains(Path.Combine("Assets", "injectedstyle.css"), styleContent);
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should work with content")]
        [PuppeteerFact]
        public async Task ShouldWorkWithContent()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await DevToolsContext.AddStyleTagAsync(new AddTagOptions { Content = "body { background-color: green; }" });
            Assert.NotNull(styleHandle as ElementHandle);
            Assert.Equal("rgb(0, 128, 0)", await DevToolsContext.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should throw when added with content to the CSP page")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => DevToolsContext.AddStyleTagAsync(new AddTagOptions
                {
                    Content = "body { background-color: green; }"
                }));
            Assert.NotNull(exception);
        }

        [PuppeteerTest("page.spec.ts", "Page.addStyleTag", "should throw when added with URL to the CSP page")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(
                () => DevToolsContext.AddStyleTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedstyle.css"
                }));
            Assert.NotNull(exception);
        }
    }
}
