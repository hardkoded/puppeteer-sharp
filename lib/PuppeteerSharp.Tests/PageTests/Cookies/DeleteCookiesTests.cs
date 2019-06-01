using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Cookies
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class DeleteCookiesTests : PuppeteerPageBaseTest
    {
        public DeleteCookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldDeleteACookie()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "cookie1",
                Value = "1"
            }, new CookieParam
            {
                Name = "cookie2",
                Value = "2"
            }, new CookieParam
            {
                Name = "cookie3",
                Value = "3"
            });
            Assert.Equal("cookie1=1; cookie2=2; cookie3=3", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            await Page.DeleteCookieAsync(new CookieParam { Name = "cookie2" });
            Assert.Equal("cookie1=1; cookie3=3", await Page.EvaluateExpressionAsync<string>("document.cookie"));
        }
    }
}