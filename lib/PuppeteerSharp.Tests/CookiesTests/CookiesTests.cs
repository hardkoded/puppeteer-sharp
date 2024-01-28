using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class CookiesTests : PuppeteerPageBaseTest
    {
        public CookiesTests() : base()
        {
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should return no cookies in pristine browser context")]
        [PuppeteerTimeout]
        public async Task ShouldReturnNoCookiesInPristineBrowserContext()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.IsEmpty(await Page.GetCookiesAsync());
        }

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get a cookie")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report httpOnly cookie")]
        [PuppeteerTimeout]
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

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report \"Strict\" sameSite cookie")]
        [PuppeteerTimeout]
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

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should properly report \"Lax\" sameSite cookie")]
        [PuppeteerTimeout]
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

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get multiple cookies")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("cookies.spec.ts", "Page.cookies", "should get cookies from multiple urls")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
    }
}
