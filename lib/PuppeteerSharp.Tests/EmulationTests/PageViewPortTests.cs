using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageViewPortTests : PuppeteerPageBaseTest
    {
        public PageViewPortTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should get the proper viewport size")]
        [PuppeteerFact]
        public async Task ShouldGetTheProperViewPortSize()
        {
            Assert.Equal(800, Page.Viewport.Width);
            Assert.Equal(600, Page.Viewport.Height);

            await Page.SetViewportAsync(new ViewPortOptions { Width = 123, Height = 456 });

            Assert.Equal(123, Page.Viewport.Width);
            Assert.Equal(456, Page.Viewport.Height);
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should support mobile emulation")]
        [PuppeteerFact]
        public async Task ShouldSupportMobileEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal(800, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));

            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.Equal(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 300 });
            Assert.Equal(400, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should support touch emulation")]
        [PuppeteerFact]
        public async Task ShouldSupportTouchEmulation()
        {
            const string dispatchTouch = @"
            function dispatchTouch() {
              let fulfill;
              const promise = new Promise(x => fulfill = x);
              window.ontouchstart = function(e) {
                fulfill('Received touch');
              };
              window.dispatchEvent(new Event('touchstart'));

              fulfill('Did not receive touch');

              return promise;
            }";

            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.False(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));

            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.True(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
            Assert.Equal("Received touch", await Page.EvaluateFunctionAsync<string>(dispatchTouch));

            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.False(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should be detectable by Modernizr")]
        [PuppeteerFact]
        public async Task ShouldBeDetectableByModernizr()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("NO", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("YES", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should detect touch when applying viewport with touches")]
        [PuppeteerFact]
        public async Task ShouldDetectTouchWhenApplyingViewportWithTouches()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetViewportAsync(new ViewPortOptions
            {
                Width = 800,
                Height = 600,
                HasTouch = true
            });
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Url = TestConstants.ServerUrl + "/modernizr.js"
            });
            Assert.True(await Page.EvaluateFunctionAsync<bool>("() => globalThis.Modernizr.touchevents"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should support landscape emulation")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportLandscapeEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewportAsync(TestConstants.IPhone6Landscape.ViewPort);
            Assert.Equal("landscape-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.Equal("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
        }
    }
}