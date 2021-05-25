using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AddScriptTagTests : PuppeteerPageBaseTest
    {
        public AddScriptTagTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = await Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddScriptTagAsync(new AddTagOptions()));
            Assert.Equal("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js" });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAUrlAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Url = "/es6/es6import.js", Type = "module" });
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAPathAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "es6", "es6pathimport.js")),
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAContentAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "import num from '/es6/es6module.js'; window.__es6injected = num;",
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => Page.AddScriptTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            Assert.Equal("Loading script from /nonexistfile.js failed", exception.Message);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            var result = await Page.EvaluateExpressionAsync<string>("__injectedError.stack");
            Assert.Contains(Path.Combine("Assets", "injectedfile.js"), result);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 35;" });
            Assert.NotNull(scriptHandle as ElementHandle);
            Assert.Equal(35, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Fact(Skip = "@see https://github.com/GoogleChrome/puppeteer/issues/4840")]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Content = "window.__injected = 35;"
                }));
            Assert.NotNull(exception);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedfile.js"
                }));
            Assert.NotNull(exception);
        }
    }
}