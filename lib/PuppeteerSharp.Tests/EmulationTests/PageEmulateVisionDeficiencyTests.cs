using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateVisionDeficiencyTests : PuppeteerPageBaseTest
    {
        public PageEmulateVisionDeficiencyTests(): base()
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateVisionDeficiency", "should work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.None);
            var screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Achromatopsia);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-achromatopsia.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.BlurredVision);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-blurredVision.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Deuteranopia);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-deuteranopia.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Protanopia);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-protanopia.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.Tritanopia);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("vision-deficiency-tritanopia.png", screenshot));

            await Page.EmulateVisionDeficiencyAsync(VisionDeficiency.None);
            screenshot = await Page.ScreenshotDataAsync();
            Assert.True(ScreenshotHelper.PixelMatch("screenshot-sanity.png", screenshot));
        }
    }
}
