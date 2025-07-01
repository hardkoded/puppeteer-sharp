using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageViewPortTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should get the proper viewport size")]
        public async Task ShouldGetTheProperViewPortSize()
        {
            Assert.That(Page.Viewport.Width, Is.EqualTo(800));
            Assert.That(Page.Viewport.Height, Is.EqualTo(600));

            await Page.SetViewportAsync(new ViewPortOptions { Width = 123, Height = 456 });

            Assert.That(Page.Viewport.Width, Is.EqualTo(123));
            Assert.That(Page.Viewport.Height, Is.EqualTo(456));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support mobile emulation")]
        public async Task ShouldSupportMobileEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.innerWidth"), Is.EqualTo(800));

            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.innerWidth"), Is.EqualTo(375));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 400, Height = 300 });
            Assert.That(await Page.EvaluateExpressionAsync<int>("window.innerWidth"), Is.EqualTo(400));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support touch emulation")]
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
            Assert.That(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"), Is.False);

            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            Assert.That(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"), Is.True);
            Assert.That(await Page.EvaluateFunctionAsync<string>(dispatchTouch), Is.EqualTo("Received touch"));

            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.That(await Page.EvaluateExpressionAsync<bool>("'ontouchstart' in window"), Is.False);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should be detectable by Modernizr")]
        public async Task ShouldBeDetectableByModernizr()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"), Is.EqualTo("NO"));
            await Page.SetViewportAsync(TestConstants.IPhone.ViewPort);
            await Page.GoToAsync(TestConstants.ServerUrl + "/detect-touch.html");
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.body.textContent.trim()"), Is.EqualTo("YES"));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should detect touch when applying viewport with touches")]
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
            Assert.That(await Page.EvaluateFunctionAsync<bool>("() => globalThis.Modernizr.touchevents"), Is.True);
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should support landscape emulation")]
        public async Task ShouldSupportLandscapeEmulation()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            Assert.That(await Page.EvaluateExpressionAsync<string>("screen.orientation.type"), Is.EqualTo("portrait-primary"));
            await Page.SetViewportAsync(TestConstants.IPhone6Landscape.ViewPort);
            Assert.That(await Page.EvaluateExpressionAsync<string>("screen.orientation.type"), Is.EqualTo("landscape-primary"));
            await Page.SetViewportAsync(new ViewPortOptions { Width = 100, Height = 100 });
            Assert.That(await Page.EvaluateExpressionAsync<string>("screen.orientation.type"), Is.EqualTo("portrait-primary"));
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should update media queries when resoltion changes")]
        public async Task ShouldUpdateMediaQueriesWhenResolutionChanges()
        {
            foreach (var dpr in new[] { 1, 2, 3 })
            {
                await Page.SetViewportAsync(new ViewPortOptions { Width = 800, Height = 600, DeviceScaleFactor = dpr });

                await Page.GoToAsync(TestConstants.ServerUrl + "/resolution.html");
                Assert.That(await GetFontSizeAsync(), Is.EqualTo(dpr));
                var screenshot = await Page.ScreenshotDataAsync(new ScreenshotOptions() { FullPage = false });
                Assert.That(ScreenshotHelper.PixelMatch($"device-pixel-ratio{dpr}.png", screenshot), Is.True);
            }
        }

        [Test, PuppeteerTest("emulation.spec", "Emulation Page.viewport", "should load correct pictures when emulation dpr")]
        public async Task ShouldLoadCorrectPicturesWhenEmulationDpr()
        {
            foreach (var dpr in new[] { 1, 2, 3 })
            {
                await Page.SetViewportAsync(new ViewPortOptions { Width = 800, Height = 600, DeviceScaleFactor = dpr });

                await Page.GoToAsync(TestConstants.ServerUrl + "/picture.html");
                Assert.That(await GetCurrentSrc(), Does.EndWith($"logo-{dpr}x.png"));

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
