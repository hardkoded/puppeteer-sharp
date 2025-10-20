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

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.isVisible and ElementHandle.isHidden", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div style='display: none'>text</div>");
            var element = await Page.WaitForSelectorAsync("div").ConfigureAwait(false);
            Assert.That(await element.IsVisibleAsync(), Is.False);
            Assert.That(await element.IsHiddenAsync(), Is.True);

            await element.EvaluateFunctionAsync("e => e.style.removeProperty('display')");
            Assert.That(await element.IsVisibleAsync(), Is.True);
            Assert.That(await element.IsHiddenAsync(), Is.False);
        }
    }
}
