using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AddStyleTagsTests : PuppeteerPageBaseTest
    {
        private const string BackgroundColorScript =
            "window.getComputedStyle(document.querySelector('body')).getPropertyValue('background-color')";

        [Fact]
        public async Task ShouldThrowAnErrorIfNoOptionsAreProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var exception = await Assert.ThrowsAsync<PuppeteerException>(() =>
                Page.AddStyleTagAsync(new AddTagOptions())
            );

            Assert.Contains("Provide an object with a `Url`, `Path` or `Content` property", exception.Message);
        }
        
        [Fact]
        public async Task ShouldWorkWithAUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions
            {
                Url = "/injectedstyle.css"
            });

            Assert.NotNull(styleHandle.AsElement());

            var bgColor = await Page.EvaluateExpressionAsync<string>(BackgroundColorScript);
            Assert.Equal("rgb(255, 0, 0)", bgColor);
        }

        [Fact]
        public async Task ShouldThrowAnErrorIfLoadingFromUrlFail()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var exception = await Assert.ThrowsAsync<PuppeteerException>(() =>
                Page.AddStyleTagAsync(new AddTagOptions
                {
                    Url = "/nonexistfile.css"
                })
            );

            Assert.Contains("Loading style from /nonexistfile.css failed", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithAPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions
            {
                Path = "assets/injectedstyle.css"
            });

            Assert.NotNull(styleHandle.AsElement());

            var bgColor = await Page.EvaluateExpressionAsync<string>(BackgroundColorScript);
            Assert.Equal("rgb(255, 0, 0)", bgColor);
        }

        [Fact]
        public async Task ShouldIncludeSourcemapWhenPathIsProvided()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.AddStyleTagAsync(new AddTagOptions
            {
                Path = "assets/injectedstyle.css"
            });

            // TODO use "page.$" implementation
            var styleHandle = await Page.EvaluateExpressionAsync("document.querySelector('style')");
            var styleContent = await Page.EvaluateFunctionAsync("(style) => style.innerHTML", styleHandle);

            Assert.Contains(Path.Combine("assets", "injectedstyle.css"), styleContent);
        }

        [Fact]
        public async Task ShouldWorkWithContent()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var styleHandle = await Page.AddStyleTagAsync(new AddTagOptions
            {
                Content = "body { background-color: green; }"
            });

            Assert.NotNull(styleHandle.AsElement());

            var bgColor = await Page.EvaluateExpressionAsync<string>(BackgroundColorScript);

            Assert.Equal("rgb(0, 128, 0)", bgColor);
        }
    }
}