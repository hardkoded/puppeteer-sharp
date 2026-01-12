using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserTests
{
    public class WindowBoundsTests : PuppeteerBrowserBaseTest
    {
        public WindowBoundsTests() : base()
        {
        }

        [Test, PuppeteerTest("browser.spec", "Browser.get|setWindowBounds", "should get and set browser window bounds")]
        public async Task ShouldGetAndSetBrowserWindowBounds()
        {
            var initialBounds = new WindowBounds
            {
                Left = 10,
                Top = 20,
                Width = 800,
                Height = 600
            };

            await using var context = await Browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            var windowId = await page.WindowIdAsync();
            await Browser.SetWindowBoundsAsync(windowId, initialBounds);
            
            var bounds = await Browser.GetWindowBoundsAsync(windowId);
            Assert.That(bounds.Left, Is.EqualTo(initialBounds.Left));
            Assert.That(bounds.Top, Is.EqualTo(initialBounds.Top));
            Assert.That(bounds.Width, Is.EqualTo(initialBounds.Width));
            Assert.That(bounds.Height, Is.EqualTo(initialBounds.Height));

            var setBounds = new WindowBounds
            {
                Left = 100,
                Top = 200,
                Width = 1600,
                Height = 1200
            };
            await Browser.SetWindowBoundsAsync(windowId, setBounds);
            
            bounds = await Browser.GetWindowBoundsAsync(windowId);
            Assert.That(bounds.Left, Is.EqualTo(setBounds.Left));
            Assert.That(bounds.Top, Is.EqualTo(setBounds.Top));
            Assert.That(bounds.Width, Is.EqualTo(setBounds.Width));
            Assert.That(bounds.Height, Is.EqualTo(setBounds.Height));
        }
    }
}
