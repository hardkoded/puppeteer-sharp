using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ClickTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
        public async Task ShouldWorkForShadowDomV1()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/shadow.html");
            var buttonHandle = (await Page.EvaluateFunctionHandleAsync("button")).AsElement();
            await buttonHandle.ClickAsync();
            Assert.True(await Page.EvaluateFunctionAsync<bool>("clicked"));
        }
    }
}
