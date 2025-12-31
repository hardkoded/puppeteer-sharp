using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    public class DefaultBrowserContextTests : PuppeteerPageBaseTest
    {
        public DefaultBrowserContextTests() : base()
        {
        }

        [SetUp]
        public async Task CreateNewPageAsync()
        {
            Context = Browser.DefaultContext;
            Page = await Context.NewPageAsync();
        }

        [Test, PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.cookies() should work")]
        public async Task PageGetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookie = (await Page.GetCookiesAsync()).First();
            Assert.That(cookie.Name, Is.EqualTo("username"));
            Assert.That(cookie.Value, Is.EqualTo("John Doe"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(16));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
        }

        [Test, PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.setCookie() should work")]
        public async Task PageSetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "username",
                Value = "John Doe"
            });

            var cookie = (await Page.GetCookiesAsync()).First();
            Assert.That(cookie.Name, Is.EqualTo("username"));
            Assert.That(cookie.Value, Is.EqualTo("John Doe"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(16));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
        }

        [Test, PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.deleteCookie() should work")]
        public async Task PageDeleteCookieAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(
                new CookieParam
                {
                    Name = "cookie1",
                    Value = "1"
                },
                new CookieParam
                {
                    Name = "cookie2",
                    Value = "2"
                });

            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("cookie1=1; cookie2=2"));
            await Page.DeleteCookieAsync(new CookieParam
            {
                Name = "cookie2"
            });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("cookie1=1"));

            var cookie = (await Page.GetCookiesAsync()).First();
            Assert.That(cookie.Name, Is.EqualTo("cookie1"));
            Assert.That(cookie.Value, Is.EqualTo("1"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(8));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
        }
    }
}
