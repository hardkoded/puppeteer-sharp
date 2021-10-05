using System;
using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CookiesTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetCookiesTests : PuppeteerPageBaseTest
    {
        public SetCookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });
            Assert.Equal("password=123456", await Page.EvaluateExpressionAsync<string>("document.cookie"));
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should set multiple cookies")]
        [PuppeteerFact]
        public async Task ShouldSetMultipleCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(
                new CookieParam
                {
                    Name = "password",
                    Value = "123456"
                },
                new CookieParam
                {
                    Name = "foo",
                    Value = "bar"
                }
            );

            Assert.Equal(
                new[]
                {
                    "foo=bar",
                    "password=123456"
                },
                await Page.EvaluateFunctionAsync<string[]>(@"() => {
                    const cookies = document.cookie.split(';');
                    return cookies.map(cookie => cookie.trim()).sort();
                }")
            );
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should have |expires| set to |-1| for session cookies")]
        [PuppeteerFact]
        public async Task ShouldHaveExpiresSetToMinus1ForSessionCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });

            var cookies = await Page.GetCookiesAsync();

            Assert.True(cookies[0].Session);
            Assert.Equal(-1, cookies[0].Expires);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should set cookie with reasonable defaults")]
        [PuppeteerFact]
        public async Task ShouldSetCookieWithReasonableDefaults()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });

            var cookie = Assert.Single(await Page.GetCookiesAsync());
            Assert.Equal("password", cookie.Name);
            Assert.Equal("123456", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(-1, cookie.Expires);
            Assert.Equal(14, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should set a cookie with a path")]
        [PuppeteerFact]
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
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should not set a cookie on a blank page")]
        [PuppeteerFact]
        public async Task ShouldNotSetACookieOnABlankPage()
        {
            await Page.GoToAsync(TestConstants.AboutBlank);

            var exception = await Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));
            Assert.Equal("Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified", exception.Message);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should not set a cookie with blank page URL")]
        [PuppeteerFact]
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

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should not set a cookie on a data URL page")]
        [PuppeteerFact]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {
            await Page.GoToAsync("data:,Hello%2C%20World!");
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));

            Assert.Equal("Protocol error (Network.deleteCookies): At least one of the url and domain needs to be specified", exception.Message);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should default to setting secure cookie for HTTPS websites")]
        [PuppeteerFact]
        public async Task ShouldDefaultToSettingSecureCookieForHttpsWebsites()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var SecureUrl = "https://example.com";

            await Page.SetCookieAsync(new CookieParam
            {
                Url = SecureUrl,
                Name = "foo",
                Value = "bar"
            });
            var cookie = Assert.Single(await Page.GetCookiesAsync(SecureUrl));
            Assert.True(cookie.Secure);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should be able to set unsecure cookie for HTTP website")]
        [PuppeteerFact]
        public async Task ShouldBeAbleToSetUnsecureCookieForHttpWebSite()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var SecureUrl = "http://example.com";

            await Page.SetCookieAsync(new CookieParam
            {
                Url = SecureUrl,
                Name = "foo",
                Value = "bar"
            });
            var cookie = Assert.Single(await Page.GetCookiesAsync(SecureUrl));
            Assert.False(cookie.Secure);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should set a cookie on a different domain")]
        [PuppeteerFact]
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

        [PuppeteerTest("cookies.spec.ts", "Page.setCookie", "should set cookies from a frame")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
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
            Assert.Equal(string.Empty, await Page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"));
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
