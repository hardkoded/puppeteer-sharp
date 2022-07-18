using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class IsIntersectingViewportTests : DevToolsContextBaseTest
    {
        public IsIntersectingViewportTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.isIntersectingViewport", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (var i = 0; i < 11; ++i)
            {
                var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("#btn" + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.Equal(visible, await button.IsIntersectingViewportAsync());
            }
        }
    }
}
