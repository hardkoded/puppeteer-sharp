using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EmulateTests : PuppeteerPageBaseTest
    {
        public EmulateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulate", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            await Page.EmulateAsync(TestConstants.IPhone);

            Assert.Equal(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            Assert.Contains("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulate", "should support clicking")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSupportClicking()
        {
            await Page.EmulateAsync(TestConstants.IPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.marginTop = '200px'", button);
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }
    }
}
