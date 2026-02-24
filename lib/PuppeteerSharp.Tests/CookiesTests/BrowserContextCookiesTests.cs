using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class BrowserContextCookiesTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.cookies", "should find no cookies in new context")]
        public async Task ShouldFindNoCookiesInNewContext()
        {
            await using var context = await Browser.CreateBrowserContextAsync();
            Assert.That(await context.GetCookiesAsync(), Is.Empty);
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.cookies", "should find cookie created in page")]
        public async Task ShouldFindCookieCreatedInPage()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("document.cookie = 'infoCookie = secret'");

            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.GreaterThan(0));
            Assert.That(cookies.Any(c => c.Name == "infoCookie" && c.Value == "secret"), Is.True);
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.cookies", "should find partitioned cookie")]
        public async Task ShouldFindPartitionedCookie()
        {
            var topLevelSite = "https://example.test";
            await Context.SetCookieAsync(new CookieData
            {
                Name = "infoCookie",
                Value = "secret",
                Domain = new Uri(topLevelSite).Host,
                Path = "/",
                Expires = -1,
                HttpOnly = false,
                Secure = true,
                PartitionKey = TestConstants.IsChrome
                    ? new CookiePartitionKey
                    {
                        SourceOrigin = topLevelSite,
                        HasCrossSiteAncestor = false,
                    }
                    : new CookiePartitionKey
                    {
                        SourceOrigin = topLevelSite,
                    },
            });
            var cookies = await Context.GetCookiesAsync();
            Assert.That(cookies, Has.Length.EqualTo(1));

            if (TestConstants.IsChrome)
            {
                Assert.That(cookies[0].PartitionKey, Is.EqualTo(new CookiePartitionKey
                {
                    SourceOrigin = topLevelSite,
                    HasCrossSiteAncestor = false,
                }));
            }
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.setCookie", "should set with undefined partition key")]
        public async Task ShouldSetWithUndefinedPartitionKey()
        {
            await Context.SetCookieAsync(new CookieData
            {
                Name = "infoCookie",
                Value = "secret",
                Domain = "localhost",
                Path = "/",
                Expires = -1,
                HttpOnly = false,
                Secure = false,
                SourceScheme = CookieSourceScheme.NonSecure,
            });

            await Page.GoToAsync(TestConstants.EmptyPage);

            Assert.That(
                await Page.EvaluateExpressionAsync<string>("document.cookie"),
                Is.EqualTo("infoCookie=secret"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.setCookie", "should set cookie with a partition key")]
        public async Task ShouldSetCookieWithAPartitionKey()
        {
            var options = TestConstants.DefaultBrowserOptions();
            options.AcceptInsecureCerts = true;

            await using var browser = await Puppeteer.LaunchAsync(options, TestConstants.LoggerFactory);
            var context = browser.DefaultContext;
            var page = await context.NewPageAsync();

            var url = new Uri(TestConstants.HttpsPrefix + "/empty.html");
            await context.SetCookieAsync(new CookieData
            {
                Name = "infoCookie",
                Value = "secret",
                Domain = url.Host,
                Secure = true,
                PartitionKey = TestConstants.IsChrome
                    ? new CookiePartitionKey
                    {
                        SourceOrigin = url.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.UriEscaped),
                        HasCrossSiteAncestor = false,
                    }
                    : new CookiePartitionKey
                    {
                        SourceOrigin = url.GetLeftPart(UriPartial.Authority),
                    },
            });

            await page.GoToAsync(url.ToString());

            Assert.That(
                await page.EvaluateExpressionAsync<string>("document.cookie"),
                Is.EqualTo("infoCookie=secret"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteCookies", "should delete cookies")]
        public async Task ShouldDeleteCookies()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Context.SetCookieAsync(
                new CookieData
                {
                    Name = "cookie1",
                    Value = "1",
                    Domain = "localhost",
                    Path = "/",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = false,
                    SourceScheme = CookieSourceScheme.NonSecure,
                },
                new CookieData
                {
                    Name = "cookie2",
                    Value = "2",
                    Domain = "localhost",
                    Path = "/",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = false,
                    SourceScheme = CookieSourceScheme.NonSecure,
                });
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("document.cookie"),
                Is.EqualTo("cookie1=1; cookie2=2"));
            await Context.DeleteCookieAsync((await Context.GetCookiesAsync()).Where(c => c.Name == "cookie1").ToArray());
            Assert.That(
                await Page.EvaluateExpressionAsync<string>("document.cookie"),
                Is.EqualTo("cookie2=2"));
        }

        [Test, PuppeteerTest("browsercontext-cookies.spec", "BrowserContext cookies BrowserContext.deleteMatchingCookies", "should delete cookies matching {\"name\":\"cookie1\"}")]
        public async Task ShouldDeleteCookiesMatchingName()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(await Context.GetCookiesAsync(), Is.Empty);
            await Context.SetCookieAsync(
                new CookieData
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "localhost",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = false,
                },
                new CookieData
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
                new CookieData
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieData
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
                new CookieData
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieData
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
                new CookieData
                {
                    Name = "cookie1",
                    Value = "secret",
                    Domain = "example.test",
                    Path = "/test",
                    Expires = -1,
                    HttpOnly = false,
                    Secure = true,
                },
                new CookieData
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
