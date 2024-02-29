using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class SetCookiesTests : PuppeteerPageBaseTest
    {
        public SetCookiesTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });
            Assert.AreEqual("password=123456", await Page.EvaluateExpressionAsync<string>("document.cookie"));
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should isolate cookies in browser contexts")]
        public async Task ShouldIsolateCookiesInBrowserContexts()
        {
            var anotherContext = await Browser.CreateBrowserContextAsync();
            var anotherPage = await anotherContext.NewPageAsync();

            await Page.GoToAsync(TestConstants.EmptyPage);
            await anotherPage.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "page1cookie",
                Value = "page1value"
            });

            await anotherPage.SetCookieAsync(new CookieParam
            {
                Name = "page2cookie",
                Value = "page2value"
            });

            var cookies1 = await Page.GetCookiesAsync();
            var cookies2 = await anotherPage.GetCookiesAsync();

            Assert.That(cookies1, Has.Exactly(1).Items);
            Assert.That(cookies2, Has.Exactly(1).Items);
            Assert.AreEqual("page1cookie", cookies1[0].Name);
            Assert.AreEqual("page1value", cookies1[0].Value);
            Assert.AreEqual("page2cookie", cookies2[0].Name);
            Assert.AreEqual("page2value", cookies2[0].Value);

            await anotherContext.CloseAsync();
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set multiple cookies")]
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

            Assert.AreEqual(
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

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should have |expires| set to |-1| for session cookies")]
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
            Assert.AreEqual(-1, cookies[0].Expires);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set cookie with reasonable defaults")]
        public async Task ShouldSetCookieWithReasonableDefaults()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456"
            });

            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("password", cookie.Name);
            Assert.AreEqual("123456", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(-1, cookie.Expires);
            Assert.AreEqual(14, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set a cookie with a path")]
        public async Task ShouldSetACookieWithAPath()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "gridcookie",
                Value = "GRID",
                Path = "/grid.html"
            });
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("gridcookie", cookie.Name);
            Assert.AreEqual("GRID", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/grid.html", cookie.Path);
            Assert.AreEqual(cookie.Expires, -1);
            Assert.AreEqual(14, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should not set a cookie on a blank page")]
        public async Task ShouldNotSetACookieOnABlankPage()
        {
            await Page.GoToAsync(TestConstants.AboutBlank);

            var exception = Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));
            StringAssert.Contains("At least one of the url and domain needs to be specified", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should not set a cookie with blank page URL")]
        public async Task ShouldNotSetACookieWithBlankPageURL()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");

            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await Page.SetCookieAsync(new CookieParam
            {
                Name = "example-cookie",
                Value = "best"
            }, new CookieParam
            {
                Url = TestConstants.AboutBlank,
                Name = "example-cookie-blank",
                Value = "best"
            }));
            Assert.AreEqual("Blank page can not have cookie \"example-cookie-blank\"", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should not set a cookie on a data URL page")]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {
            await Page.GoToAsync("data:,Hello%2C%20World!");
            var exception = Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));

            StringAssert.Contains("At least one of the url and domain needs to be specified", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should default to setting secure cookie for HTTPS websites")]
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

            var cookies = await Page.GetCookiesAsync(SecureUrl);
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.True(cookie.Secure);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should be able to set insecure cookie for HTTP website")]
        public async Task ShouldDefaultToSettingSecureCookieForHttpWebsites()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var SecureUrl = "http://example.com";

            await Page.SetCookieAsync(new CookieParam
            {
                Url = SecureUrl,
                Name = "foo",
                Value = "bar"
            });

            var cookies = await Page.GetCookiesAsync(SecureUrl);
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.False(cookie.Secure);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should be able to set unsecure cookie for HTTP website")]
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
            var cookies = await Page.GetCookiesAsync(SecureUrl);
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.False(cookie.Secure);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set a cookie on a different domain")]
        public async Task ShouldSetACookieOnADifferentDomain()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best", Url = "https://www.example.com" });
            Assert.AreEqual(string.Empty, await Page.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.IsEmpty(await Page.GetCookiesAsync());
            var cookies = await Page.GetCookiesAsync("https://www.example.com");
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("example-cookie", cookie.Name);
            Assert.AreEqual("best", cookie.Value);
            Assert.AreEqual("www.example.com", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(cookie.Expires, -1);
            Assert.AreEqual(18, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set cookies from a frame")]
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
            Assert.AreEqual("localhost-cookie=best", await Page.EvaluateExpressionAsync<string>("document.cookie"));
            Assert.AreEqual(string.Empty, await Page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"));
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("localhost-cookie", cookie.Name);
            Assert.AreEqual("best", cookie.Value);
            Assert.AreEqual("localhost", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(cookie.Expires, -1);
            Assert.AreEqual(20, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);

            cookies = await Page.GetCookiesAsync(TestConstants.CrossProcessHttpPrefix);
            Assert.That(cookies, Has.Exactly(1).Items);
            cookie = cookies.First();
            Assert.AreEqual("127-cookie", cookie.Name);
            Assert.AreEqual("worst", cookie.Value);
            Assert.AreEqual("127.0.0.1", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(cookie.Expires, -1);
            Assert.AreEqual(15, cookie.Size);
            Assert.False(cookie.HttpOnly);
            Assert.False(cookie.Secure);
            Assert.True(cookie.Session);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set secure same-site cookies from a frame")]
        public async Task ShouldSetSecureSameSiteCookiesFromAFrame()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.IgnoreHTTPSErrors = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync(TestConstants.HttpsPrefix + "/grid.html");
            await page.EvaluateFunctionAsync(@"src => {
                    let fulfill;
                    const promise = new Promise(x => fulfill = x);
                    const iframe = document.createElement('iframe');
                    document.body.appendChild(iframe);
                    iframe.onload = fulfill;
                    iframe.src = src;
                    return promise;
                }", TestConstants.CrossProcessHttpsPrefix + "/grid.html");
            await page.SetCookieAsync(
                new CookieParam
                {
                    Name = "127-cookie",
                    Value = "best",
                    Url = TestConstants.CrossProcessHttpsPrefix + "/grid.html",
                    SameSite = SameSite.None,
                });
            Assert.AreEqual("127-cookie=best", await page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"));
            var cookies = await page.GetCookiesAsync(TestConstants.CrossProcessHttpsPrefix + "/grid.html");
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.AreEqual("127-cookie", cookie.Name);
            Assert.AreEqual("best", cookie.Value);
            Assert.AreEqual("127.0.0.1", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(cookie.Expires, -1);
            Assert.AreEqual(14, cookie.Size);

            // Puppeteer uses expectCookieEquals which excludes SameSite attribute
            if (TestConstants.IsChrome)
            {
                Assert.AreEqual(SameSite.None, cookie.SameSite);
            }
            Assert.False(cookie.HttpOnly);
            Assert.True(cookie.Secure);
            Assert.True(cookie.Session);
        }
    }
}
