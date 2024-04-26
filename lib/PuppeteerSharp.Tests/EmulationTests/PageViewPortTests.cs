using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageViewPortTests : PuppeteerPageBaseTest
    {
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

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should update media queries when resoltion changes")]
        public async Task ShouldUpdateMediaQueriesWhenResolutionChanges()
        {
            foreach (var dpr in new[] { 1, 2, 3 })
            {
                await Page.SetViewportAsync(new ViewPortOptions { Width = 800, Height = 600, DeviceScaleFactor = dpr });

                await Page.GoToAsync(TestConstants.ServerUrl + "/resolution.html");
                Assert.AreEqual(dpr, await GetFontSizeAsync());
                var screenshot = await Page.ScreenshotDataAsync(new ScreenshotOptions() { FullPage = false });
                Assert.True(ScreenshotHelper.PixelMatch($"device-pixel-ratio{dpr}.png", screenshot));
            }
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should load correct pictures when emulation dpr")]
        public async Task ShouldLoadCorrectPicturesWhenEmulationDpr()
        {
            foreach (var dpr in new[] { 1, 2, 3 })
            {
                await Page.SetViewportAsync(new ViewPortOptions { Width = 800, Height = 600, DeviceScaleFactor = dpr });

                await Page.GoToAsync(TestConstants.ServerUrl + "/picture.html");
                StringAssert.EndsWith($"logo-{dpr}x.png", await GetCurrentSrc());

            }
        }

        private Task<int> GetFontSizeAsync()
            => Page.EvaluateFunctionAsync<int>(@"() => {
                return parseInt(window.getComputedStyle(document.querySelector('p')).fontSize, 10);
            }");

        private Task<string> GetCurrentSrc()
            => Page.EvaluateFunctionAsync<string>(@"() => {
                return document.querySelector('img').currentSrc;
            }");
    }
}
