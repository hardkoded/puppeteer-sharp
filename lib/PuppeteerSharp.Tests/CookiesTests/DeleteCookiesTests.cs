using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CookiesTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DeleteCookiesTests : DevToolsContextBaseTest
    {
        public DeleteCookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("cookies.spec.ts", "Page.deleteCookie", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await DevToolsContext.SetCookieAsync(new CookieParam
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
            Assert.Equal("cookie1=1; cookie2=2; cookie3=3", await DevToolsContext.EvaluateExpressionAsync<string>("document.cookie"));
            await DevToolsContext.DeleteCookieAsync(new CookieParam { Name = "cookie2" });
            Assert.Equal("cookie1=1; cookie3=3", await DevToolsContext.EvaluateExpressionAsync<string>("document.cookie"));
        }
    }
}
