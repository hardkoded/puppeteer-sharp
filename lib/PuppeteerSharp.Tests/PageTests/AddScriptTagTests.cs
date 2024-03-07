using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class AddScriptTagTests : PuppeteerPageBaseTest
    {
        public AddScriptTagTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw an error if no options are provided")]
        public void ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddScriptTagAsync(new AddTagOptions()));
            Assert.AreEqual("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a url")]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js" });
            Assert.NotNull(scriptHandle);
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a url and type=module")]
        public async Task ShouldWorkWithAUrlAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Url = "/es6/es6import.js", Type = "module" });
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a path and type=module")]
        public async Task ShouldWorkWithAPathAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "es6", "es6pathimport.js")),
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a content and type=module")]
        public async Task ShouldWorkWithAContentAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "import num from '/es6/es6module.js'; window.__es6injected = num;",
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("__es6injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw an error if loading from url fail")]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(()
                => Page.AddScriptTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            StringAssert.Contains("Could not load script", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a path")]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            Assert.NotNull(scriptHandle);
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should include sourcemap when path is provided")]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            var result = await Page.EvaluateExpressionAsync<string>("__injectedError.stack");
            StringAssert.Contains(Path.Combine("Assets", "injectedfile.js"), result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with content")]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 35;" });
            Assert.NotNull(scriptHandle);
            Assert.AreEqual(35, await Page.EvaluateExpressionAsync<int>("__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should add id when provided")]
        public async Task ShouldAddIdWhenProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 1;", Id = "one" });
            await Page.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js", Id = "two" });

            Assert.NotNull(await Page.QuerySelectorAsync("#one"));
            Assert.NotNull(await Page.QuerySelectorAsync("#two"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw when added with content to the CSP page")]
        [Ignore("@see https://github.com/GoogleChrome/puppeteer/issues/4840")]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Content = "window.__injected = 35;"
                }));
            Assert.NotNull(exception);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw when added with URL to the CSP page")]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedfile.js"
                }));
            Assert.NotNull(exception);
        }
    }
}
