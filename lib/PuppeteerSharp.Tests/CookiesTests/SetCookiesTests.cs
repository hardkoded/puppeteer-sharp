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
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("password=123456"));
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
            Assert.That(cookies1[0].Name, Is.EqualTo("page1cookie"));
            Assert.That(cookies1[0].Value, Is.EqualTo("page1value"));
            Assert.That(cookies2[0].Name, Is.EqualTo("page2cookie"));
            Assert.That(cookies2[0].Value, Is.EqualTo("page2value"));

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

            Assert.That(
                await Page.EvaluateFunctionAsync<string[]>(@"() => {
                    const cookies = document.cookie.split(';');
                    return cookies.map(cookie => cookie.trim()).sort();
                }")
, Is.EqualTo(new[]
                {
                    "foo=bar",
                    "password=123456"
                }));
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

            Assert.That(cookies[0].Session, Is.True);
            Assert.That(cookies[0].Expires, Is.EqualTo(-1));
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
            Assert.That(cookie.Name, Is.EqualTo("password"));
            Assert.That(cookie.Value, Is.EqualTo("123456"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(14));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set cookie with all available properties")]
        public async Task ShouldSetCookieWithAllAvailableProperties()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            await Page.SetCookieAsync(new CookieParam
            {
                Name = "password",
                Value = "123456",
                Domain = "localhost",
                Path = "/",
                SameParty = false,
                Expires = -1,
                HttpOnly = false,
                Secure = false,
                SourceScheme = CookieSourceScheme.Unset,
            });

            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.That(cookie.Name, Is.EqualTo("password"));
            Assert.That(cookie.Value, Is.EqualTo("123456"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(14));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
            Assert.That(cookie.SourceScheme, Is.EqualTo(CookieSourceScheme.Unset));
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
            Assert.That(cookie.Name, Is.EqualTo("gridcookie"));
            Assert.That(cookie.Value, Is.EqualTo("GRID"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/grid.html"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(14));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should not set a cookie on a blank page")]
        public async Task ShouldNotSetACookieOnABlankPage()
        {
            await Page.GoToAsync(TestConstants.AboutBlank);

            var exception = Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));
            Assert.That(exception.Message, Does.Contain("At least one of the url and domain needs to be specified"));
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
            Assert.That(exception.Message, Is.EqualTo("Blank page can not have cookie \"example-cookie-blank\""));
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should not set a cookie on a data URL page")]
        public async Task ShouldNotSetACookieOnADataURLPage()
        {
            await Page.GoToAsync("data:,Hello%2C%20World!");
            var exception = Assert.ThrowsAsync<MessageException>(async () => await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best" }));

            Assert.That(exception.Message, Does.Contain("At least one of the url and domain needs to be specified"));
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
            Assert.That(cookie.Secure, Is.True);
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
            Assert.That(cookie.Secure, Is.False);
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
            Assert.That(cookie.Secure, Is.False);
        }

        [Test, Retry(2), PuppeteerTest("cookies.spec", "Cookie specs Page.setCookie", "should set a cookie on a different domain")]
        public async Task ShouldSetACookieOnADifferentDomain()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam { Name = "example-cookie", Value = "best", Url = "https://www.example.com" });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo(string.Empty));
            Assert.That(await Page.GetCookiesAsync(), Is.Empty);
            var cookies = await Page.GetCookiesAsync("https://www.example.com");
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.That(cookie.Name, Is.EqualTo("example-cookie"));
            Assert.That(cookie.Value, Is.EqualTo("best"));
            Assert.That(cookie.Domain, Is.EqualTo("www.example.com"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(18));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.True);
            Assert.That(cookie.Session, Is.True);
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
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("localhost-cookie=best"));
            Assert.That(await Page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo(string.Empty));
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.That(cookie.Name, Is.EqualTo("localhost-cookie"));
            Assert.That(cookie.Value, Is.EqualTo("best"));
            Assert.That(cookie.Domain, Is.EqualTo("localhost"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(20));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);

            cookies = await Page.GetCookiesAsync(TestConstants.CrossProcessHttpPrefix);
            Assert.That(cookies, Has.Exactly(1).Items);
            cookie = cookies.First();
            Assert.That(cookie.Name, Is.EqualTo("127-cookie"));
            Assert.That(cookie.Value, Is.EqualTo("worst"));
            Assert.That(cookie.Domain, Is.EqualTo("127.0.0.1"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(15));
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.False);
            Assert.That(cookie.Session, Is.True);
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
            Assert.That(await page.FirstChildFrame().EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("127-cookie=best"));
            var cookies = await page.GetCookiesAsync(TestConstants.CrossProcessHttpsPrefix + "/grid.html");
            Assert.That(cookies, Has.Exactly(1).Items);
            var cookie = cookies.First();
            Assert.That(cookie.Name, Is.EqualTo("127-cookie"));
            Assert.That(cookie.Value, Is.EqualTo("best"));
            Assert.That(cookie.Domain, Is.EqualTo("127.0.0.1"));
            Assert.That(cookie.Path, Is.EqualTo("/"));
            Assert.That(cookie.Expires, Is.EqualTo(-1));
            Assert.That(cookie.Size, Is.EqualTo(14));

            // Puppeteer uses expectCookieEquals which excludes SameSite attribute
            if (TestConstants.IsChrome)
            {
                Assert.That(cookie.SameSite, Is.EqualTo(SameSite.None));
            }
            Assert.That(cookie.HttpOnly, Is.False);
            Assert.That(cookie.Secure, Is.True);
            Assert.That(cookie.Session, Is.True);
        }
    }
}
