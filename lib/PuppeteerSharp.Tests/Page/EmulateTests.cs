﻿using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EmulateTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            await Page.EmulateAsync(TestConstants.IPhone);

            Assert.Equal(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            Assert.Contains("Safari", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [Fact]
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
