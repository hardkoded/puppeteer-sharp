using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class ElementHandleQuerySelectorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await Page.SetContentAsync(TestUtils.Html(@"<html>
          <body>
            <div class=""second""><div class=""inner"">A</div></div>
          </body>
        </html>"));
            var htmlEl = await Page.QuerySelectorAsync("html");
            var second = await htmlEl.QuerySelectorAsync(".second");
            var inner = await second.QuerySelectorAsync(".inner");
            var content = await Page.EvaluateFunctionAsync<string>("e => e.textContent", inner);
            Assert.That(content, Is.EqualTo("A"));
        }

        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync(TestUtils.Html(@"<html>
          <body>
            <div class=""second""><div class=""inner"">B</div></div>
          </body>
        </html>"));
            var htmlEl = await Page.QuerySelectorAsync("html");
            var second = await htmlEl.QuerySelectorAsync(".third");
            Assert.That(second, Is.Null);
        }

        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$$ xpath", "should query existing element")]
        public async Task XPathShouldQueryExistingElement()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await Page.SetContentAsync(TestUtils.Html(@"<html>
            <body>
              <div class=""second""><div class=""inner"">A</div></div>
            </body>
          </html>"));
            var htmlEl = await Page.QuerySelectorAsync("html");
            var second = await htmlEl.QuerySelectorAllAsync("xpath/./body/div[contains(@class, 'second')]");
            var inner = await second[0].QuerySelectorAllAsync("xpath/./div[contains(@class, 'inner')]");
            var content = await Page.EvaluateFunctionAsync<string>("e => e.textContent", inner[0]);
            Assert.That(content, Is.EqualTo("A"));
        }

        [Test, PuppeteerTest("queryselector.spec", "ElementHandle.$$ xpath", "should return null for non-existing element")]
        public async Task XPathShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync(TestUtils.Html(@"<html>
            <body>
              <div class=""second""><div class=""inner"">B</div></div>
            </body>
          </html>"));
            var htmlEl = await Page.QuerySelectorAsync("html");
            var second = await htmlEl.QuerySelectorAllAsync("xpath/div[contains(@class, 'third')]");
            Assert.That(second, Is.Empty);
        }
    }
}
