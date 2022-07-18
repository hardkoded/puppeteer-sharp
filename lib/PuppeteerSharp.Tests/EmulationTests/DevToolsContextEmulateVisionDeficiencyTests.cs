using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextEmulateVisionDeficiencyTests : DevToolsContextBaseTest
    {
        public DevToolsContextEmulateVisionDeficiencyTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateVisionDeficiency", "should work")]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.None);
            var screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.Achromatopsia);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-achromatopsia.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.BlurredVision);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-blurredVision.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.Deuteranopia);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-deuteranopia.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.Protanopia);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-protanopia.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.Tritanopia);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-tritanopia.png", screenshot));

            await DevToolsContext.EmulateVisionDeficiencyAsync(VisionDeficiency.None);
            screenshot = await DevToolsContext.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));
        }
    }
}
