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

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw an error if no options are provided")]
        public void ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddScriptTagAsync(new AddTagOptions()));
            Assert.That(exception.Message, Is.EqualTo("Provide options with a `Url`, `Path` or `Content` property"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a url")]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js" });
            Assert.That(scriptHandle, Is.Not.Null);
            Assert.That(await Page.EvaluateExpressionAsync<int>("__injected"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a url and type=module")]
        public async Task ShouldWorkWithAUrlAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Url = "/es6/es6import.js", Type = "module" });
            Assert.That(await Page.EvaluateExpressionAsync<int>("__es6injected"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a path and type=module")]
        public async Task ShouldWorkWithAPathAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "es6", "es6pathimport.js")),
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.That(await Page.EvaluateExpressionAsync<int>("__es6injected"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a content and type=module")]
        public async Task ShouldWorkWithAContentAndTypeModule()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "import num from '/es6/es6module.js'; window.__es6injected = num;",
                Type = "module"
            });
            await Page.WaitForFunctionAsync("() => window.__es6injected");
            Assert.That(await Page.EvaluateExpressionAsync<int>("__es6injected"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw an error if loading from url fail")]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(()
                => Page.AddScriptTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            Assert.That(exception.Message, Does.Contain("Could not load script"));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with a path")]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            Assert.That(scriptHandle, Is.Not.Null);
            Assert.That(await Page.EvaluateExpressionAsync<int>("__injected"), Is.EqualTo(42));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should include sourcemap when path is provided")]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedfile.js"))
            });
            var result = await Page.EvaluateExpressionAsync<string>("__injectedError.stack");
            Assert.That(result, Does.Contain(Path.Combine("Assets", "injectedfile.js")));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should work with content")]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var scriptHandle = await Page.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 35;" });
            Assert.That(scriptHandle, Is.Not.Null);
            Assert.That(await Page.EvaluateExpressionAsync<int>("__injected"), Is.EqualTo(35));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should add id when provided")]
        public async Task ShouldAddIdWhenProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions { Content = "window.__injected = 1;", Id = "one" });
            await Page.AddScriptTagAsync(new AddTagOptions { Url = "/injectedfile.js", Id = "two" });

            Assert.That(await Page.QuerySelectorAsync("#one"), Is.Not.Null);
            Assert.That(await Page.QuerySelectorAsync("#two"), Is.Not.Null);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw when added with content to the CSP page")]
        [Ignore("@see https://github.com/GoogleChrome/puppeteer/issues/4840")]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Content = "window.__injected = 35;"
                }));
            Assert.That(exception, Is.Not.Null);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.addScriptTag", "should throw when added with URL to the CSP page")]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddScriptTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedfile.js"
                }));
            Assert.That(exception, Is.Not.Null);
        }
    }
}
