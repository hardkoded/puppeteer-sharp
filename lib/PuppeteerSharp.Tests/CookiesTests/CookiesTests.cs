using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.DevTools.Dom;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.CookiesTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CookiesTests : DevToolsContextBaseTest
    {
        public CookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should return no cookies in pristine browser context")]
        [PuppeteerFact]
        public async Task ShouldReturnNoCookiesInPristineBrowserContext()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(await DevToolsContext.GetCookiesAsync());
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get a cookie")]
        [PuppeteerFact]
        public async Task ShouldGetACookie()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(await DevToolsContext.GetCookiesAsync());

            await DevToolsContext.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookie = Assert.Single(await DevToolsContext.GetCookiesAsync());
            Assert.Equal("username", cookie.Name);
            Assert.Equal("John Doe", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report httpOnly cookie")]
        [PuppeteerFact]
        public async Task ShouldProperlyReportHttpOnlyCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; HttpOnly; Path=/";
                return Task.CompletedTask;
            });
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var cookies = await DevToolsContext.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.True(cookies[0].HttpOnly);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report \"Strict\" sameSite cookie")]
        [PuppeteerFact]
        public async Task ShouldProperlyReportSStrictSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; SameSite=Strict";
                return Task.CompletedTask;
            });
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var cookies = await DevToolsContext.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.Equal(SameSite.Strict, cookies[0].SameSite);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report \"Lax\" sameSite cookie")]
        [PuppeteerFact]
        public async Task ShouldProperlyReportLaxSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; SameSite=Lax";
                return Task.CompletedTask;
            });
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var cookies = await DevToolsContext.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.Equal(SameSite.Lax, cookies[0].SameSite);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get multiple cookies")]
        [PuppeteerFact]
        public async Task ShouldGetMultipleCookies()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(await DevToolsContext.GetCookiesAsync());

            await DevToolsContext.EvaluateFunctionAsync(@"() => {
                document.cookie = 'username=John Doe';
                document.cookie = 'password=1234';
            }");

            var cookies = (await DevToolsContext.GetCookiesAsync()).OrderBy(c => c.Name).ToList();

            var cookie = cookies[0];
            Assert.Equal("password", cookie.Name);
            Assert.Equal("1234", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(12, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);

            cookie = cookies[1];
            Assert.Equal("username", cookie.Name);
            Assert.Equal("John Doe", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get cookies from multiple urls")]
        [PuppeteerFact]
        public async Task ShouldGetCookiesFromMultipleUrls()
        {
            await DevToolsContext.SetCookieAsync(
                new CookieParam
                {
                    Url = "https://foo.com",
                    Name = "doggo",
                    Value = "woofs"
                },
                new CookieParam
                {
                    Url = "https://bar.com",
                    Name = "catto",
                    Value = "purrs"
                },
                new CookieParam
                {
                    Url = "https://baz.com",
                    Name = "birdo",
                    Value = "tweets"
                }
            );
            var cookies = (await DevToolsContext.GetCookiesAsync("https://foo.com", "https://baz.com")).OrderBy(c => c.Name).ToList();

            Assert.Equal(2, cookies.Count);

            var cookie = cookies[0];
            Assert.Equal("birdo", cookie.Name);
            Assert.Equal("tweets", cookie.Value);
            Assert.Equal("baz.com", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(11, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);

            cookie = cookies[1];
            Assert.Equal("doggo", cookie.Name);
            Assert.Equal("woofs", cookie.Value);
            Assert.Equal("foo.com", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(cookie.Expires, -1);
            Assert.Equal(10, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);
        }
    }
}
