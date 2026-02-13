using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class AddRemoveScreenTests : PuppeteerBrowserBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.add|removeScreen", "should add and remove a screen")]
        public async Task ShouldAddAndRemoveAScreen()
        {
            var screenInfo = await Browser.AddScreenAsync(new AddScreenParams
            {
                Left = 800,
                Top = 0,
                Width = 1600,
                Height = 1200,
                ColorDepth = 32,
                WorkAreaInsets = new WorkAreaInsets { Bottom = 80 },
                Label = "secondary",
            });

            Assert.That(screenInfo.AvailHeight, Is.EqualTo(1120));
            Assert.That(screenInfo.AvailLeft, Is.EqualTo(800));
            Assert.That(screenInfo.AvailTop, Is.EqualTo(0));
            Assert.That(screenInfo.AvailWidth, Is.EqualTo(1600));
            Assert.That(screenInfo.ColorDepth, Is.EqualTo(32));
            Assert.That(screenInfo.DevicePixelRatio, Is.EqualTo(1));
            Assert.That(screenInfo.Height, Is.EqualTo(1200));
            Assert.That(screenInfo.Id, Is.EqualTo("2"));
            Assert.That(screenInfo.IsExtended, Is.True);
            Assert.That(screenInfo.IsInternal, Is.False);
            Assert.That(screenInfo.IsPrimary, Is.False);
            Assert.That(screenInfo.Label, Is.EqualTo("secondary"));
            Assert.That(screenInfo.Left, Is.EqualTo(800));
            Assert.That(screenInfo.Orientation.Angle, Is.EqualTo(0));
            Assert.That(screenInfo.Orientation.Type, Is.EqualTo("landscapePrimary"));
            Assert.That(screenInfo.Top, Is.EqualTo(0));
            Assert.That(screenInfo.Width, Is.EqualTo(1600));

            var screens = await Browser.ScreensAsync();
            Assert.That(screens, Has.Length.EqualTo(2));

            await Browser.RemoveScreenAsync(screenInfo.Id);

            screens = await Browser.ScreensAsync();
            Assert.That(screens, Has.Length.EqualTo(1));
        }
    }
}
