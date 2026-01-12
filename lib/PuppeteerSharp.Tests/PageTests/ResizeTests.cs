using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class ResizeTests : PuppeteerPageBaseTest
    {
        public ResizeTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.resize", "should resize the window content area")]
        public async Task ShouldResizeWindowContentArea()
        {
            // Navigate to a page first
            await Page.GoToAsync(TestConstants.EmptyPage);

            // Resize the window content area
            var newWidth = 800;
            var newHeight = 600;
            await Page.ResizeAsync(newWidth, newHeight);

            // Get the window inner dimensions after resize
            var dimensions = await Page.EvaluateFunctionAsync<WindowDimensions>(@"() => {
                return {
                    width: window.innerWidth,
                    height: window.innerHeight
                };
            }");

            // Verify the content area was resized
            Assert.That(dimensions.Width, Is.EqualTo(newWidth));
            Assert.That(dimensions.Height, Is.EqualTo(newHeight));
        }

        [Test, PuppeteerTest("page.spec", "Page Page.windowId", "should return window id")]
        public async Task ShouldReturnWindowId()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var windowId = await Page.WindowIdAsync();

            Assert.That(windowId, Is.Not.Null);
            Assert.That(windowId, Is.Not.Empty);
        }

        private sealed class WindowDimensions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
