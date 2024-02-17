using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class IsIntersectingViewportTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTimeout, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isIntersectingViewport", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/offscreenbuttons.html");
            for (var i = 0; i < 11; ++i)
            {
                var button = await Page.QuerySelectorAsync("#btn" + i);
                // All but last button are visible.
                var visible = i < 10;
                Assert.AreEqual(visible, await button.IsIntersectingViewportAsync());
            }
        }
    }
}
