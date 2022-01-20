using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ElementHandleQuerySelectorTests : PuppeteerPageBaseTest
    {
        public ElementHandleQuerySelectorTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/playground.html");
            await DevToolsContext.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">A</div></div></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAsync(".second");
            var inner = await second.QuerySelectorAsync(".inner");
            var content = await DevToolsContext.EvaluateFunctionAsync<string>("e => e.textContent", inner);
            Assert.Equal("A", content);
        }

        [PuppeteerTest("queryselector.spec.ts", "ElementHandle.$", "should return null for non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await DevToolsContext.SetContentAsync("<html><body><div class=\"second\"><div class=\"inner\">B</div></div></body></html>");
            var html = await DevToolsContext.QuerySelectorAsync("html");
            var second = await html.QuerySelectorAsync(".third");
            Assert.Null(second);
        }
    }
}