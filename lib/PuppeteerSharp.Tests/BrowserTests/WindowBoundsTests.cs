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

            var initialBounds = new WindowBounds
            {
                Left = 10,
                Top = 20,
                Width = 800,
                Height = 600,
            };
            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Type = CreatePageType.Window,
                WindowBounds = initialBounds,
            });

            var windowId = await page.WindowIdAsync();
            var bounds = await browser.GetWindowBoundsAsync(windowId);
            Assert.That(bounds.Left, Is.EqualTo(10));
            Assert.That(bounds.Top, Is.EqualTo(20));
            Assert.That(bounds.Width, Is.EqualTo(800));
            Assert.That(bounds.Height, Is.EqualTo(600));

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

        [Test, PuppeteerTest("browser.spec", "Browser specs Browser.get|setWindowBounds", "should set and get browser window maximized state")]
        public async Task ShouldSetAndGetBrowserWindowMaximizedState()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();

            // Add a secondary screen.
            var screenInfo = await browser.AddScreenAsync(new AddScreenParams
            {
                Left = 800,
                Top = 0,
                Width = 1600,
                Height = 1200,
            });

            // Open a window on the secondary screen.
            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Type = CreatePageType.Window,
                WindowBounds = new WindowBounds
                {
                    Left = screenInfo.AvailLeft + 50,
                    Top = screenInfo.AvailTop + 50,
                    Width = screenInfo.AvailWidth - 100,
                    Height = screenInfo.AvailHeight - 100,
                },
            });

            // Maximize the created window.
            var windowId = await page.WindowIdAsync();
            await browser.SetWindowBoundsAsync(windowId, new WindowBounds { WindowState = WindowState.Maximized });

            // Expect the maximized window to fill the entire screen's available area.
            var bounds = await browser.GetWindowBoundsAsync(windowId);
            Assert.That(bounds.Left, Is.EqualTo(screenInfo.AvailLeft));
            Assert.That(bounds.Top, Is.EqualTo(screenInfo.AvailTop));
            Assert.That(bounds.Width, Is.EqualTo(screenInfo.AvailWidth));
            Assert.That(bounds.Height, Is.EqualTo(screenInfo.AvailHeight));
            Assert.That(bounds.WindowState, Is.EqualTo(WindowState.Maximized));

            // Cleanup.
            await browser.RemoveScreenAsync(screenInfo.Id);
        }
    }
}
