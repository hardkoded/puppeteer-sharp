using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Cookies
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CookiesTests : PuppeteerPageBaseTest
    {
        public CookiesTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldReturnNoCookiesInPristineBrowserContext()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(await Page.GetCookiesAsync());
        }

        [Fact]
        public async Task ShouldGetACookie()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
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
        }

        [Fact]
        public async Task ShouldProperlyReportHttpOnlyCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = ";HttpOnly; Path=/";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.True(cookies[0].HttpOnly);
        }

        [Fact]
        public async Task ShouldProperlyReportSStrictSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = ";SameSite=Strict";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.Equal(SameSite.Strict, cookies[0].SameSite);
        }

        [Fact]
        public async Task ShouldProperlyReportLaxSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = ";SameSite=Lax";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.Single(cookies);
            Assert.Equal(SameSite.Lax, cookies[0].SameSite);
        }

        [Fact]
        public async Task ShouldGetMultipleCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Empty(await Page.GetCookiesAsync());

            await Page.EvaluateFunctionAsync(@"() => {
                document.cookie = 'username=John Doe';
                document.cookie = 'password=1234';
            }");

            var cookies = (await Page.GetCookiesAsync()).OrderBy(c => c.Name).ToList();

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

        [Fact]
        public async Task ShouldGetCookiesFromMultipleUrls()
        {
            await Page.SetCookieAsync(
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
            var cookies = (await Page.GetCookiesAsync("https://foo.com", "https://baz.com")).OrderBy(c => c.Name).ToList();

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