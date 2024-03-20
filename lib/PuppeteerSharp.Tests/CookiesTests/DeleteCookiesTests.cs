using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class DeleteCookiesTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should delete cookie")]
        public async Task ShouldDeleteCookie()
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
            Assert.AreEqual("cookie1=1; cookie2=2; cookie3=3", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            await Page.DeleteCookieAsync(new CookieParam { Name = "cookie2" });
            Assert.AreEqual("cookie1=1; cookie3=3", await Page.EvaluateExpressionAsync<string>("document.cookie"));
        }
    }
}
