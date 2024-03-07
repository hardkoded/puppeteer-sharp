using System;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class AddStyleTagTests : PuppeteerPageBaseTest
    {
        public AddStyleTagTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should throw an error if no options are provided")]
        public void ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            var exception = Assert.ThrowsAsync<ArgumentException>(()
                => Page.AddStyleTagAsync(new AddTagOptions()));
            Assert.AreEqual("Provide options with a `Url`, `Path` or `Content` property", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should work with a url")]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Url = "/injectedstyle.css" });
            Assert.NotNull(styleHandle);
            Assert.AreEqual("rgb(255, 0, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should throw an error if loading from url fail")]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(()
                => Page.AddStyleTagAsync(new AddTagOptions { Url = "/nonexistfile.js" }));
            StringAssert.Contains("Could not load style", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should work with a path")]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Path = "Assets/injectedstyle.css" });
            Assert.NotNull(styleHandle);
            Assert.AreEqual("rgb(255, 0, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should include sourcemap when path is provided")]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddStyleTagAsync(new AddTagOptions
            {
                Path = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("Assets", "injectedstyle.css"))
            });
            var styleHandle = await Page.QuerySelectorAsync("style");
            var styleContent = await Page.EvaluateFunctionAsync<string>("style => style.innerHTML", styleHandle);
            StringAssert.Contains(Path.Combine("Assets", "injectedstyle.css"), styleContent);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should work with content")]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions { Content = "body { background-color: green; }" });
            Assert.NotNull(styleHandle);
            Assert.AreEqual("rgb(0, 128, 0)", await Page.EvaluateExpressionAsync<string>(
                "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should throw when added with content to the CSP page")]
        public async Task ShouldThrowWhenAddedWithContentToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddStyleTagAsync(new AddTagOptions
                {
                    Content = "body { background-color: green; }"
                }));
            Assert.NotNull(exception);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.addStyleTag", "should throw when added with URL to the CSP page")]
        public async Task ShouldThrowWhenAddedWithURLToTheCSPPage()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            var exception = Assert.ThrowsAsync<EvaluationFailedException>(
                () => Page.AddStyleTagAsync(new AddTagOptions
                {
                    Url = TestConstants.CrossProcessUrl + "/injectedstyle.css"
                }));
            Assert.NotNull(exception);
        }
    }
}
