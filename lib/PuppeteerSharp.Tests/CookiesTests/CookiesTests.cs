using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class CookiesTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should return no cookies in pristine browser context")]
        public async Task ShouldReturnNoCookiesInPristineBrowserContext()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.IsEmpty(await Page.GetCookiesAsync());
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should get a cookie")]
        public async Task ShouldGetACookie()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.IsEmpty(await Page.GetCookiesAsync());

            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("username", cookie.Name);
            Assert.AreEqual("John Doe", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should properly report httpOnly cookie")]
        public async Task ShouldProperlyReportHttpOnlyCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; HttpOnly; Path=/";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            Assert.True(cookies[0].HttpOnly);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should properly report \"Strict\" sameSite cookie")]
        public async Task ShouldProperlyReportSStrictSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; SameSite=Strict";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            Assert.AreEqual(SameSite.Strict, cookies[0].SameSite);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should properly report \"Lax\" sameSite cookie")]
        public async Task ShouldProperlyReportLaxSameSiteCookie()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["Set-Cookie"] = "a=b; SameSite=Lax";
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.EmptyPage);
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            Assert.AreEqual(SameSite.Lax, cookies[0].SameSite);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should get multiple cookies")]
        public async Task ShouldGetMultipleCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.IsEmpty(await Page.GetCookiesAsync());

            await Page.EvaluateFunctionAsync(@"() => {
                document.cookie = 'username=John Doe';
                document.cookie = 'password=1234';
            }");

            var cookies = (await Page.GetCookiesAsync()).OrderBy(c => c.Name).ToList();

            var cookie = cookies[0];
            Assert.AreEqual("password", cookie.Name);
            Assert.AreEqual("1234", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(12, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);

            cookie = cookies[1];
            Assert.AreEqual("username", cookie.Name);
            Assert.AreEqual("John Doe", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should get cookies from multiple urls")]
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

            Assert.AreEqual(2, cookies.Count);

            var cookie = cookies[0];
            Assert.AreEqual("birdo", cookie.Name);
            Assert.AreEqual("tweets", cookie.Value);
            Assert.AreEqual("baz.com", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(11, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);

            cookie = cookies[1];
            Assert.AreEqual("doggo", cookie.Name);
            Assert.AreEqual("woofs", cookie.Value);
            Assert.AreEqual("foo.com", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(10, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should not get cookies from subdomain")]
        public async Task ShouldNotGetCookiesFromSubdomain()
        {
            await Page.SetCookieAsync(new CookieParam
            {
                Url = "https://base_domain.com",
                Name = "doggo",
                Value = "woofs"
            });
            Assert.IsEmpty(await Page.GetCookiesAsync("https://sub_domain.base_domain.com"));
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should get cookies from nested path")]
        public async Task ShouldGetCookiesFromNestedPath()
        {
            await Page.SetCookieAsync(new CookieParam
            {
                Url = "https://foo.com",
                Path = "/some_path",
                Name = "doggo",
                Value = "woofs"
            });

            var cookies = await Page.GetCookiesAsync("https://foo.com/some_path/nested_path");

            Assert.That(cookies, Has.Exactly(1).Items);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.cookies", "should not get cookies from not nested path")]
        public async Task ShouldNotGetCookiesFromNotNestedPath()
        {
            await Page.SetCookieAsync(new CookieParam
            {
                Url = "https://foo.com",
                Path = "/some_path",
                Name = "doggo",
                Value = "woofs"
            });

            Assert.IsEmpty(await Page.GetCookiesAsync("https://foo.com/some_path_looks_like_nested"));
        }
    }
}
