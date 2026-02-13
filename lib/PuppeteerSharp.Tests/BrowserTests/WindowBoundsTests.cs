using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class WindowBoundsTests : PuppeteerBrowserContextBaseTest
    {
        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.get|setWindowBounds", "should get and set browser window bounds")]
        public async Task ShouldGetAndSetBrowserWindowBounds()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            var windowId = await page.WindowIdAsync();
            var bounds = await browser.GetWindowBoundsAsync(windowId);
            Assert.That(bounds, Is.Not.Null);

            var setBounds = new WindowBounds
            {
                Left = 100,
                Top = 200,
                Width = 1600,
                Height = 1200,
            };
            await browser.SetWindowBoundsAsync(windowId, setBounds);
            var newBounds = await browser.GetWindowBoundsAsync(windowId);
            Assert.That(newBounds.Left, Is.EqualTo(100));
            Assert.That(newBounds.Top, Is.EqualTo(200));
            Assert.That(newBounds.Width, Is.EqualTo(1600));
            Assert.That(newBounds.Height, Is.EqualTo(1200));
        }
    }
}
