using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class NewPageTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.newPage", "should open pages in a new window")]
        public async Task ShouldOpenPagesInANewWindow()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();

            // Open an initial page in the context so we have a target already.
            await context.NewPageAsync();

            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Type = CreatePageType.Window,
            });

            var windowId = await page.WindowIdAsync();
            Assert.That(windowId, Is.Not.Null);
            Assert.That(windowId, Is.Not.Empty);
        }

        [Test, PuppeteerTest("page.spec", "Page Page.newPage", "should open pages in a new window at the specified position")]
        public async Task ShouldOpenPagesInANewWindowAtTheSpecifiedPosition()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();

            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Type = CreatePageType.Window,
                WindowBounds = new WindowBounds
                {
                    Left = 50,
                    Top = 50,
                    Width = 750,
                    Height = 550,
                },
            });

            var outerSize = await page.EvaluateFunctionAsync<OuterSize>(
                "() => ({ width: window.outerWidth, height: window.outerHeight })");

            Assert.That(outerSize.Width, Is.EqualTo(750));
            Assert.That(outerSize.Height, Is.EqualTo(550));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.newPage", "should open pages in a new window in maximized state")]
        public async Task ShouldOpenPagesInANewWindowInMaximizedState()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.DefaultViewport = null;

            await using var browser = await Puppeteer.LaunchAsync(options);
            var context = await browser.CreateBrowserContextAsync();

            var page = await context.NewPageAsync(new CreatePageOptions
            {
                Type = CreatePageType.Window,
                WindowBounds = new WindowBounds
                {
                    WindowState = WindowState.Maximized,
                },
            });

            var outerSize = await page.EvaluateFunctionAsync<OuterSize>(
                "() => ({ width: window.outerWidth, height: window.outerHeight })");

            // Default headless screen is 800x600.
            Assert.That(outerSize.Width, Is.EqualTo(800));
            Assert.That(outerSize.Height, Is.EqualTo(600));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.newPage", "should create a background page")]
        public async Task ShouldCreateABackgroundPage()
        {
            var page = await Context.NewPageAsync(new CreatePageOptions { Background = true });

            var visibilityState = await page.EvaluateExpressionAsync<string>("document.visibilityState");
            Assert.That(visibilityState, Is.EqualTo("hidden"));
        }

        private sealed class OuterSize
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }
    }
}
