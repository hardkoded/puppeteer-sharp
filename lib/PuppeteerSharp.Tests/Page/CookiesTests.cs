using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CookiesTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldGetAndSetCookies()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Empty(await page.GetCookiesAsync());

            await page.EvaluateFunctionAsync(@"() =>
            {
                document.cookie = 'username=John Doe';
            }");
            var cookie = Assert.Single(await page.GetCookiesAsync());
            Assert.Equal(cookie.Name, "username");
            Assert.Equal(cookie.Value, "John Doe");
            Assert.Equal(cookie.Domain, "localhost");
            Assert.Equal(cookie.Path, "/");
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(cookie.Size, 16);
            Assert.Equal(cookie.HttpOnly, false);
            Assert.Equal(cookie.Secure, false);
            Assert.Equal(cookie.Session, true);

            await page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });
            Assert.Equal("username=John Doe; password=123456", await page.EvaluateExpressionAsync<string>("document.cookie"));
            var cookies = (await page.GetCookiesAsync()).OrderBy(c => c.Name).ToList();
            Assert.Equal(2, cookies.Count);

            Assert.Equal(cookies[0].Name, "password");
            Assert.Equal(cookies[0].Value, "123456");
            Assert.Equal(cookies[0].Domain, "localhost");
            Assert.Equal(cookies[0].Path, "/");
            Assert.Equal(cookies[0].Expires, -1);
            Assert.Equal(cookies[0].Size, 14);
            Assert.Equal(cookies[0].HttpOnly, false);
            Assert.Equal(cookies[0].Secure, false);
            Assert.Equal(cookies[0].Session, true);

            Assert.Equal(cookies[1].Name, "username");
            Assert.Equal(cookies[1].Value, "John Doe");
            Assert.Equal(cookies[1].Domain, "localhost");
            Assert.Equal(cookies[1].Path, "/");
            Assert.Equal(cookies[1].Expires, -1);
            Assert.Equal(cookies[1].Size, 16);
            Assert.Equal(cookies[1].HttpOnly, false);
            Assert.Equal(cookies[1].Secure, false);
            Assert.Equal(cookies[1].Session, true);
        }

        [Fact]
        public async Task ShouldSetACookieWithAPath()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await page.SetCookieAsync(new CookieParam
            {
                Name = "gridcookie",
                Value = "GRID",
                Path = "/grid.html"
            });
            var cookie = Assert.Single(await page.GetCookiesAsync());
            Assert.Equal(cookie.Name, "gridcookie");
            Assert.Equal(cookie.Value, "GRID");
            Assert.Equal(cookie.Domain, "localhost");
            Assert.Equal(cookie.Path, "/grid.html");
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(cookie.Size, 14);
            Assert.Equal(cookie.HttpOnly, false);
            Assert.Equal(cookie.Secure, false);
            Assert.Equal(cookie.Session, true);
            Assert.Equal("gridcookie=GRID", await page.EvaluateExpressionAsync<string>("document.cookie"));

            await page.GoToAsync(TestConstants.ServerUrl + "/empty.html");
            Assert.Empty(await page.GetCookiesAsync());
            Assert.Equal(string.Empty, await page.EvaluateExpressionAsync<string>("document.cookie"));

            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Equal("gridcookie=GRID", await page.EvaluateExpressionAsync<string>("document.cookie"));
        }

        [Fact]
        public async Task ShouldDeleteACookie()
        {
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await page.SetCookieAsync(new CookieParam
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
            Assert.Equal("cookie1=1; cookie2=2; cookie3=3", await page.EvaluateExpressionAsync<string>("document.cookie"));
            await page.DeleteCookieAsync(new CookieParam { Name = "cookie2" });
            Assert.Equal("cookie1=1; cookie3=3", await page.EvaluateExpressionAsync<string>("document.cookie"));
        }

        [Fact]
        public async Task ShouldNotSetACookieOnABlankPage()
        {

        }

        [Fact]
        public async Task ShouldNotSetACookieWithBlankPageURL()
        {

        }

        [Fact]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {

        }

        [Fact] // need a better name for this one
        public async Task ShouldNotSetACookieWithBlankPageURL2()
        {

        }

        [Fact]
        public async Task ShouldSetACookieOnADifferentDomain()
        {

        }

        [Fact]
        public async Task ShouldSetCookiesFromAFrame()
        {

        }
    }
}
