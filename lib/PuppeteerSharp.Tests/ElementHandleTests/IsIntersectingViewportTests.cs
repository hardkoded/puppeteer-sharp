using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class IsIntersectingViewportTests : PuppeteerPageBaseTest
    {
        public IsIntersectingViewportTests(): base()
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.isIntersectingViewport", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (var i = 0; i < 11; ++i)
            {
                var button = await Page.QuerySelectorAsync("#btn" + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.Equal(visible, await button.IsIntersectingViewportAsync());
            }
        }
    }
}