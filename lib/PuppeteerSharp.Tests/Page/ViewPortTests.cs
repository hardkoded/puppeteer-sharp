using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ViewPortTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldGetTheProperViewPortSize()
        {
            Assert.Equal(800, Page.Viewport.Width);
            Assert.Equal(600, Page.Viewport.Height);

            await Page.SetViewport(new ViewPortOptions { Width = 123, Height = 456 });

            Assert.Equal(123, Page.Viewport.Width);
            Assert.Equal(456, Page.Viewport.Height);
        }

        [Fact]
        public async Task ShouldSupportMobileEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal(800, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));

            await Page.SetViewport(TestConstants.IPhone.ViewPort);
            Assert.Equal(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            await Page.SetViewport(new ViewPortOptions { Width = 400, Height = 300 });
            Assert.Equal(400, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
        }

        [Fact]
        public async Task ShouldSupportTouchEmulation()
        {
            const string dispatchTouch = @"
            function dispatchTouch() {
              let fulfill;
              const promise = new Promise(x => fulfill = x);
              window.ontouchstart = function(e) {
                fulfill('Recieved touch');
              };
              window.dispatchEvent(new Event('touchstart'));

              fulfill('Did not recieve touch');

              return promise;
            }";

            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
            await Page.SetViewport(TestConstants.IPhone.ViewPort);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
            Assert.Equal("Recieved touch", await Page.EvaluateFunctionAsync<string>(dispatchTouch));
            await Page.SetViewport(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.False(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
        }

        [Fact]
        public async Task ShouldBeDetectableByModernizr()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("NO", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            await Page.SetViewport(TestConstants.IPhone.ViewPort);
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("YES", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [Fact]
        public async Task ShouldSupportLandscapeEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewport(TestConstants.IPhone6Landscape.ViewPort);
            Assert.Equal("landscape-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewport(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.Equal("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
        }
    }
}
