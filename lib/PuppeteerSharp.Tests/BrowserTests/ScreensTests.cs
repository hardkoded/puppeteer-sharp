using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class ScreensTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.screens", "should return default screen info")]
        public async Task ShouldReturnDefaultScreenInfo()
        {
            var screenInfos = await Browser.ScreensAsync();
            Assert.That(screenInfos, Has.Length.EqualTo(1));
            Assert.That(screenInfos[0].AvailHeight, Is.EqualTo(600));
            Assert.That(screenInfos[0].AvailLeft, Is.EqualTo(0));
            Assert.That(screenInfos[0].AvailTop, Is.EqualTo(0));
            Assert.That(screenInfos[0].AvailWidth, Is.EqualTo(800));
            Assert.That(screenInfos[0].ColorDepth, Is.EqualTo(24));
            Assert.That(screenInfos[0].DevicePixelRatio, Is.EqualTo(1));
            Assert.That(screenInfos[0].Height, Is.EqualTo(600));
            Assert.That(screenInfos[0].Id, Is.EqualTo("1"));
            Assert.That(screenInfos[0].IsExtended, Is.False);
            Assert.That(screenInfos[0].IsInternal, Is.False);
            Assert.That(screenInfos[0].IsPrimary, Is.True);
            Assert.That(screenInfos[0].Label, Is.EqualTo(""));
            Assert.That(screenInfos[0].Left, Is.EqualTo(0));
            Assert.That(screenInfos[0].Orientation.Angle, Is.EqualTo(0));
            Assert.That(screenInfos[0].Orientation.Type, Is.EqualTo("landscapePrimary"));
            Assert.That(screenInfos[0].Top, Is.EqualTo(0));
            Assert.That(screenInfos[0].Width, Is.EqualTo(800));
        }
    }
}
