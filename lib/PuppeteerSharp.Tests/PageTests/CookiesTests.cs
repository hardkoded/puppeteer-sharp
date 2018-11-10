using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class CookiesTests : PuppeteerPageBaseTest
    {
        public CookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldGetAndSetCookies()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Empty(await Page.GetCookiesAsync());

            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookie = Assert.Single(await Page.GetCookiesAsync());
            Assert.Equal("username", cookie.Name);
            Assert.Equal("John Doe", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });
            Assert.Equal("username=John Doe; password=123456", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            var cookies = (await Page.GetCookiesAsync()).OrderBy(c => c.Name).ToList();
            Assert.Equal(2, cookies.Count);

            Assert.Equal("password", cookies[0].Name);
            Assert.Equal("123456", cookies[0].Value);
            Assert.Equal("localhost", cookies[0].Domain);
            Assert.Equal("/", cookies[0].Path);
            Assert.Equal(cookies[0].Expires, -1);
            Assert.Equal(14, cookies[0].Size);
            Assert.False(cookies[0].HttpOnly);
            Assert.False(cookies[0].Secure);
            Assert.True(cookies[0].Session);

            Assert.Equal("username", cookies[1].Name);
            Assert.Equal("John Doe", cookies[1].Value);
            Assert.Equal("localhost", cookies[1].Domain);
            Assert.Equal("/", cookies[1].Path);
            Assert.Equal(cookies[1].Expires, -1);
            Assert.Equal(16, cookies[1].Size);
            Assert.False(cookies[1].HttpOnly);
            Assert.False(cookies[1].Secure);
            Assert.True(cookies[1].Session);
        }

        [Fact]
        public async Task ShouldSetACookieWithAPath()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "gridcookie",
                Value = "GRID",
                Path = "/grid.html"
            });
            var cookie = Assert.Single(await Page.GetCookiesAsync());
            Assert.Equal("gridcookie", cookie.Name);
            Assert.Equal("GRID", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/grid.html", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(14, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
            Assert.Equal("gridcookie=GRID", await Page.EvaluateExpressionAsync<string>("document.cookie"));

            await Page.GoToAsync(TestConstants.ServerUrl + "/empty.html");
            Assert.Empty(await Page.GetCookiesAsync());
            Assert.Equal(string.Empty, await Page.EvaluateExpressionAsync<string>("document.cookie"));

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Equal("gridcookie=GRID", await Page.EvaluateExpressionAsync<string>("document.cookie"));
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

        [Fact]
        public async Task ShouldNotSetACookieOnABlankPage()
        {
            await Page.GoToAsync(TestConstants.AboutBlank);

            var exception = await Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));
            Assert.Equal("Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified", exception.Message);
        }

        [Fact]
        public async Task ShouldNotSetACookieWithBlankPageURL()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.SetCookieAsync(new CookieParam
            {
                Name = "example-cookie",
                Value = "best"
            }, new CookieParam
            {
                Url = TestConstants.AboutBlank,
                Name = "example-cookie-blank",
                Value = "best"
            }));
            Assert.Equal("Blank page can not have cookie \"example-cookie-blank\"", exception.Message);
        }

        [Fact]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {
            await Page.GoToAsync("data:,Hello%2C%20World!");
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));

            Assert.Equal("Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified", exception.Message);
        }

        [Fact]
        public async Task ShouldSetACookieOnADifferentDomain()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best", Url = "https://www.example.com" });
            Assert.Equal(string.Empty, await Page.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.Empty(await Page.GetCookiesAsync());
            var cookie = Assert.Single(await Page.GetCookiesAsync("https://www.example.com"));
            Assert.Equal("example-cookie", cookie.Name);
            Assert.Equal("best", cookie.Value);
            Assert.Equal("www.example.com", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(18, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Fact]
        public async Task ShouldSetCookiesFromAFrame()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam { Name = "localhost-cookie", Value = "best" });
            await Page.EvaluateFunctionAsync(@"src => {
                    let fulfill;
                    const promise = new Promise(x => fulfill = x);
                    const iframe = document.createElement('iframe');
                    document.body.appendChild(iframe);
                    iframe.onload = fulfill;
                    iframe.src = src;
                    return promise;
                }", TestConstants.CrossProcessHttpPrefix);
            await Page.SetCookieAsync(new CookieParam { Name = "127-cookie", Value = "worst", Url = TestConstants.CrossProcessHttpPrefix });
            Assert.Equal("localhost-cookie=best", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.Equal("127-cookie=worst", await Page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"));
            var cookie = Assert.Single(await Page.GetCookiesAsync());
            Assert.Equal("localhost-cookie", cookie.Name);
            Assert.Equal("best", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(20, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);

            cookie = Assert.Single(await Page.GetCookiesAsync(TestConstants.CrossProcessHttpPrefix));
            Assert.Equal("127-cookie", cookie.Name);
            Assert.Equal("worst", cookie.Value);
            Assert.Equal("127.0.0.1", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(15, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }
    }
}