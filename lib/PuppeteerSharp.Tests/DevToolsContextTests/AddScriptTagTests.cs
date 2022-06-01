using System;
using System.IO;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AddScriptTagTests : PuppeteerPageBaseTest
    {
        public AddScriptTagTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should throw an error if no options are provided")]
        [PuppeteerFact]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(()
                => DevToolsContext.AddScriptTagAsync(new AddTagOptions()));
            Assert.Equal("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with a url")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAUrl()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await DevToolsContext.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js" });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with a url and type=module")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAUrlAndTypeModule()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions { Url = "/es6/es6import.js", Type = "module" });
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with a path and type=module")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAPathAndTypeModule()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "es6", "es6pathimport.js")),
                Type = "module"
            });
            await DevToolsContext.WaitForFunctionAsync("() => window.__es6injected");
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with a content and type=module")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAContentAndTypeModule()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "import num from '/es6/es6module.js'; window.__es6injected = num;",
                Type = "module"
            });
            await DevToolsContext.WaitForFunctionAsync("() => window.__es6injected");
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should throw an error if loading from url fail")]
        [PuppeteerFact]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => DevToolsContext.AddScriptTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            Assert.Equal("Loading script from /nonexistfile.js failed", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with a path")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAPath()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should include sourcemap when path is provided")]
        [PuppeteerFact]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            var result = await DevToolsContext.EvaluateExpressionAsync<string>("__injectedError.stack");
            Assert.Contains(Path.Combine("Assets", "injectedfile.js"), result);
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should work with content")]
        [PuppeteerFact]
        public async Task ShouldWorkWithContent()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await DevToolsContext.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 35;" });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(35, await DevToolsContext.EvaluateExpressionAsync<int>("__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should throw when added with content to the CSP page")]
        [PuppeteerFact(Skip = "@see https://github.com/GoogleChrome/puppeteer/issues/4840")]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => DevToolsContext.AddScriptTagAsync(new AddTagOptions
                {
                    Content = "window.__injected = 35;"
                }));
            Assert.NotNull(exception);
        }

        [PuppeteerTest("page.spec.ts", "Page.addScriptTag", "should throw when added with URL to the CSP page")]
        [PuppeteerFact]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(
                () => DevToolsContext.AddScriptTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedfile.js"
                }));
            Assert.NotNull(exception);
        }
    }
}
