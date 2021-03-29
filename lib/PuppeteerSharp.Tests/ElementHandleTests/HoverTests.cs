using System;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class HoverTests : PuppeteerPageBaseTest
    {
        public HoverTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.hover", "should work")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            var button = await Page.QuerySelectorAsync("#button-6");
            await button.HoverAsync();
            Assert.Equal("button-6", await Page.EvaluateExpressionAsync<string>(
                "document.querySelector('button:hover').id"));
        }
    }
}
