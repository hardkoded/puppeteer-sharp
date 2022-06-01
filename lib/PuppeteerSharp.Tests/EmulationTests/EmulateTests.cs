using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateTests : DevToolsContextBaseTest
    {
        public EmulateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulate", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            await DevToolsContext.EmulateAsync(TestConstants.IPhone);

            Assert.Equal(375, await DevToolsContext.EvaluateExpressionAsync<int>("window.innerWidth"));
            Assert.Contains("iPhone", await DevToolsContext.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulate", "should support clicking")]
        [PuppeteerFact]
        public async Task ShouldSupportClicking()
        {
            await DevToolsContext.EmulateAsync(TestConstants.IPhone);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await DevToolsContext.EvaluateFunctionAsync("button => button.style.marginTop = '200px'", button);
            await button.ClickAsync();
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }
    }
}
