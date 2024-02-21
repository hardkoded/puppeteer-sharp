using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class IsVisibleIsHiddenTests : PuppeteerPageBaseTest
    {
        public IsVisibleIsHiddenTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isVisible and ElementHandle.isHidden", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div style='display: none'>text</div>");
            var element = await Page.WaitForSelectorAsync("div").ConfigureAwait(false);
            Assert.False(await element.IsVisibleAsync());
            Assert.True(await element.IsHiddenAsync());

            await element.EvaluateFunctionAsync("e => e.style.removeProperty('display')");
            Assert.True(await element.IsVisibleAsync());
            Assert.False(await element.IsHiddenAsync());
        }
    }
}
