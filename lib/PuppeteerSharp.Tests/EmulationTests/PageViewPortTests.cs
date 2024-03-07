using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageViewPortTests : PuppeteerPageBaseTest
    {
        public PageViewPortTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should get the proper viewport size")]
        public async Task ShouldGetTheProperViewPortSize()
        {
            Assert.AreEqual(800, Page.Viewport.Width);
            Assert.AreEqual(600, Page.Viewport.Height);

            await Page.SetViewportAsync(new ViewPortOptions { Width = 123, Height = 456 });

            Assert.AreEqual(123, Page.Viewport.Width);
            Assert.AreEqual(456, Page.Viewport.Height);
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support mobile emulation")]
        public async Task ShouldSupportMobileEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.AreEqual(800, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));

            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.AreEqual(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 300 });
            Assert.AreEqual(400, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support touch emulation")]
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
            Assert.AreEqual("Received touch", await Page.EvaluateFunctionAsync<string>(dispatchTouch));

            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.False(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"));
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should be detectable by Modernizr")]
        public async Task ShouldBeDetectableByModernizr()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.AreEqual("NO", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.AreEqual("YES", await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should detect touch when applying viewport with touches")]
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

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support landscape emulation")]
        public async Task ShouldSupportLandscapeEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.AreEqual("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewportAsync(TestConstants.IPhone6Landscape.ViewPort);
            Assert.AreEqual("landscape-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.AreEqual("portrait-primary", await Page.EvaluateExpressionAsync<string>("screen.orientation.type"));
        }
    }
}
