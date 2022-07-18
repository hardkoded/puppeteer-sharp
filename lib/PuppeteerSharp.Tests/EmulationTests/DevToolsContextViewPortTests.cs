using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextViewPortTests : DevToolsContextBaseTest
    {
        public DevToolsContextViewPortTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should get the proper viewport size")]
        [PuppeteerFact(Skip = "TODO: OFFSCREEN DOESN'T SUPPORT RESIZE")]
        public async Task ShouldGetTheProperViewPortSize()
        {
            Assert.Equal(800, DevToolsContext.Viewport.Width);
            Assert.Equal(600, DevToolsContext.Viewport.Height);

            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 123, Height = 456 });

            Assert.Equal(123, DevToolsContext.Viewport.Width);
            Assert.Equal(456, DevToolsContext.Viewport.Height);
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should support mobile emulation")]
        [PuppeteerFact(Skip = "TODO: OFFSCREEN DOESN'T SUPPORT RESIZE")]
        public async Task ShouldSupportMobileEmulation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal(800, await DevToolsContext.EvaluateExpressionAsync<int>("window.innerWidth"));

            await DevToolsContext.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.Equal(375, await DevToolsContext.EvaluateExpressionAsync<int>("window.innerWidth"));
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 300 });
            Assert.Equal(400, await DevToolsContext.EvaluateExpressionAsync<int>("window.innerWidth"));
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

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));

            await DevToolsContext.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.True(await DevToolsContext.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
            Assert.Equal("Received touch", await DevToolsContext.EvaluateFunctionAsync<string>(dispatchTouch));

            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.False(await DevToolsContext.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should be detectable by Modernizr")]
        [PuppeteerFact]
        public async Task ShouldBeDetectableByModernizr()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("NO", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            await DevToolsContext.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.Equal("YES", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should detect touch when applying viewport with touches")]
        [PuppeteerFact]
        public async Task ShouldDetectTouchWhenApplyingViewportWithTouches()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions
            {
                Width = 800,
                Height = 600,
                HasTouch = true
            });
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Url = TestConstants.ServerUrl + "/modernizr.js"
            });
            Assert.True(await DevToolsContext.EvaluateFunctionAsync<bool>("() => Modernizr.touchevents"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.viewport", "should support landscape emulation")]
        [PuppeteerFact(Skip = "TODO: OFFSCREEN DOESN'T SUPPORT RESIZE")]
        public async Task ShouldSupportLandscapeEmulation()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.Equal("portrait-primary", await DevToolsContext.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await DevToolsContext.SetViewportAsync(TestConstants.IPhone6Landscape.ViewPort);
            Assert.Equal("landscape-primary", await DevToolsContext.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.Equal("portrait-primary", await DevToolsContext.EvaluateExpressionAsync<string>("screen.orientation.type"));
        }
    }
}
