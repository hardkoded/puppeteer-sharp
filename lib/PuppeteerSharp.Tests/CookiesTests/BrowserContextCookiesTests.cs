using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class BrowserContextCookiesTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteMatchingCookies", "should delete cookies matching {\"name\":\"cookie1\"}")]
        public async Task ShouldDeleteCookiesMatchingName()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Context.GetCookiesAsync(), Is.Empty);
            await Context.SetCookieAsync(
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "localhost",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = false,
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "secret",
                    Domain = "localhost",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = false,
                });
            Assert.That(await Context.GetCookiesAsync(), Has.Length.EqualTo(2));
            await Context.DeleteMatchingCookiesAsync(new DeleteCookiesRequest
            {
                Name = "cookie1",
            });
            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.EqualTo(1));
            Assert.That(cookies[0].Name, Is.EqualTo("cookie2"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteMatchingCookies", "should delete cookies matching {\"url\":\"https://example.test/test\",\"name\":\"cookie1\"}")]
        public async Task ShouldDeleteCookiesMatchingUrl()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Context.GetCookiesAsync(), Is.Empty);
            await Context.SetCookieAsync(
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                });
            Assert.That(await Context.GetCookiesAsync(), Has.Length.EqualTo(2));
            await Context.DeleteMatchingCookiesAsync(new DeleteCookiesRequest
            {
                Url = "https://example.test/test",
                Name = "cookie1",
            });
            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.EqualTo(1));
            Assert.That(cookies[0].Name, Is.EqualTo("cookie2"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteMatchingCookies", "should delete cookies matching {\"domain\":\"example.test\",\"name\":\"cookie1\"}")]
        public async Task ShouldDeleteCookiesMatchingDomain()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Context.GetCookiesAsync(), Is.Empty);
            await Context.SetCookieAsync(
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                });
            Assert.That(await Context.GetCookiesAsync(), Has.Length.EqualTo(2));
            await Context.DeleteMatchingCookiesAsync(new DeleteCookiesRequest
            {
                Domain = "example.test",
                Name = "cookie1",
            });
            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.EqualTo(1));
            Assert.That(cookies[0].Name, Is.EqualTo("cookie2"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteMatchingCookies", "should delete cookies matching {\"path\":\"/test\",\"name\":\"cookie1\"}")]
        public async Task ShouldDeleteCookiesMatchingPath()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Context.GetCookiesAsync(), Is.Empty);
            await Context.SetCookieAsync(
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                });
            Assert.That(await Context.GetCookiesAsync(), Has.Length.EqualTo(2));
            await Context.DeleteMatchingCookiesAsync(new DeleteCookiesRequest
            {
                Path = "/test",
                Name = "cookie1",
            });
            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.EqualTo(1));
            Assert.That(cookies[0].Name, Is.EqualTo("cookie2"));
        }
    }
}
