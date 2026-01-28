using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageResizeTests : PuppeteerPageBaseTest
    {
        public PageResizeTests() : base() { }

        [Test, PuppeteerTest("page.spec", "Page Page.resize", "should resize the browser window to fit page content")]
        public async Task ShouldResizeTheBrowserWindowToFitPageContent()
        {
            // Default viewport restricts window to 800x600, so remove it.
            await Page.SetViewportAsync(null);

            const int contentWidth = 500;
            const int contentHeight = 400;

            var resized = Page.EvaluateFunctionAsync(@"() => {
                return new Promise(resolve => {
                    window.onresize = resolve;
                });
            }");

            await Page.ResizeAsync(new ResizeOptions
            {
                ContentWidth = contentWidth,
                ContentHeight = contentHeight,
            });

            await resized;

            var innerSize = await Page.EvaluateFunctionAsync<InnerSize>(@"() => {
                return { width: window.innerWidth, height: window.innerHeight };
            }");

            Assert.That(innerSize.Width, Is.EqualTo(contentWidth));
            Assert.That(innerSize.Height, Is.EqualTo(contentHeight));
        }

        private sealed class InnerSize
        {
            public int Width { get; set; }

            public int Height { get; set; }
        }
    }
}
