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

        [Test, PuppeteerTest("page.spec", "Page Page.resize", "should resize the browser window to fit page content")]
        public async Task ShouldResizeTheBrowserWindowToFitPageContent()
        {
            // Default view port restricts window to 800x600, so remove it.
            await Page.SetViewportAsync(null);

            var contentWidth = 500;
            var contentHeight = 400;
            var resized = Page.EvaluateFunctionAsync(@"() => {
                return new Promise(resolve => {
                    window.onresize = resolve;
                });
            }");
            await Page.ResizeAsync(contentWidth, contentHeight);
            await resized;

            var innerSize = await Page.EvaluateFunctionAsync<WindowDimensions>(@"() => {
                return {width: window.innerWidth, height: window.innerHeight};
            }");
            Assert.That(innerSize.Width, Is.EqualTo(contentWidth));
            Assert.That(innerSize.Height, Is.EqualTo(contentHeight));
        }

        private sealed class WindowDimensions
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }
    }
}
