using System;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HoverTests : DevToolsContextBaseTest
    {
        public HoverTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.hover", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            var button = await DevToolsContext.QuerySelectorAsync<HtmlButtonElement>("#button-6");
            await button.HoverAsync();
            Assert.Equal("button-6", await DevToolsContext.EvaluateExpressionAsync<string>(
                "document.querySelector('button:hover').id"));
        }
    }
}
