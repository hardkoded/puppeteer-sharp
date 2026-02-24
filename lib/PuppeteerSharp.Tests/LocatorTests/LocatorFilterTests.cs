using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorFilterTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator.prototype.filter", "should resolve as soon as the predicate matches")]
        public async Task ShouldResolveAsSoonAsThePredicateMatches()
        {
            await Page.SetContentAsync("<div id='test'>test</div>");
            var hoverTask = Page
                .Locator("#test")
                .SetTimeout(5000)
                .Filter("async (element) => element.getAttribute('clickable') === 'true'")
                .Filter("(element) => element.getAttribute('clickable') === 'true'")
                .HoverAsync();

            await Task.Delay(2000);
            await Page.EvaluateExpressionAsync(
                "document.querySelector('#test')?.setAttribute('clickable', 'true')");

            await hoverTask;
        }

    }
}
