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

        [Test, Retry(2), PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.cookies() should work")]
        public async Task PageGetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookie = (await Page.GetCookiesAsync()).First();
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

        [Test, Retry(2), PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.setCookie() should work")]
        public async Task PageSetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "username",
                Value = "John Doe"
            });

            var cookie = (await Page.GetCookiesAsync()).First();
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

        [Test, Retry(2), PuppeteerTest("defaultbrowsercontext.spec", "DefaultBrowserContext", "page.deleteCookie() should work")]
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

            Assert.AreEqual("cookie1=1; cookie2=2", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            await Page.DeleteCookieAsync(new CookieParam
            {
                Name = "cookie2"
            });
            Assert.AreEqual("cookie1=1", await Page.EvaluateExpressionAsync<string>("document.cookie"));

            var cookie = (await Page.GetCookiesAsync()).First();
            Assert.AreEqual("cookie1", cookie.Name);
            Assert.AreEqual("1", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(8, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }
    }
}
