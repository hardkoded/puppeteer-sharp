using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DefaultBrowserContextTests : PuppeteerPageBaseTest
    {
        public DefaultBrowserContextTests(ITestOutputHelper output) : base(output)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Context = Browser.DefaultContext;
            Page = await Context.NewPageAsync();
        }

        [Fact]
        public async Task PageGetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var cookie = (await Page.GetCookiesAsync()).FirstOrDefault();
            Assert.Equal("username", cookie.Name);
            Assert.Equal("John Doe", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(-1, cookie.Expires);
            Assert.Equal(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Fact]
        public async Task PageSetCookiesAsyncShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "username",
                Value = "John Doe"
            });

            var cookie = (await Page.GetCookiesAsync()).FirstOrDefault();
            Assert.Equal("username", cookie.Name);
            Assert.Equal("John Doe", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(-1, cookie.Expires);
            Assert.Equal(16, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Fact]
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

            Assert.Equal("cookie1=1; cookie2=2", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            await Page.DeleteCookieAsync(new CookieParam
            {
                Name = "cookie2"
            });
            Assert.Equal("cookie1=1", await Page.EvaluateExpressionAsync<string>("document.cookie"));

            var cookie = (await Page.GetCookiesAsync()).FirstOrDefault();
            Assert.Equal("cookie1", cookie.Name);
            Assert.Equal("1", cookie.Value);
            Assert.Equal("localhost", cookie.Domain);
            Assert.Equal("/", cookie.Path);
            Assert.Equal(-1, cookie.Expires);
            Assert.Equal(8, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }
    }
}