using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.CookiesTests
{
    public class DeleteCookiesTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should delete cookie")]
        public async Task ShouldDeleteCookie()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "cookie1",
                Value = "1"
            }, new CookieParam
            {
                Name = "cookie2",
                Value = "2"
            }, new CookieParam
            {
                Name = "cookie3",
                Value = "3"
            });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("cookie1=1; cookie2=2; cookie3=3"));
            await Page.DeleteCookieAsync(new CookieParam { Name = "cookie2" });
            Assert.That(await Page.EvaluateExpressionAsync<string>("document.cookie"), Is.EqualTo("cookie1=1; cookie3=3"));
        }

        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should not delete cookie for different domain")]
        public async Task ShouldNotDeleteCookieForDifferentDomain()
        {
            const string cookieDestinationUrl = "https://example.com";
            const string cookieName = "some_cookie_name";

            await Page.GoToAsync(TestConstants.EmptyPage);

            // Set a cookie for the current page.
            await Page.SetCookieAsync(new CookieParam
            {
                Name = cookieName,
                Value = "local page cookie value",
            });
            Assert.That(await Page.GetCookiesAsync(), Has.Exactly(1).Items);

            // Set a cookie for different domain.
            await Page.SetCookieAsync(new CookieParam
            {
                Url = cookieDestinationUrl,
                Name = cookieName,
                Value = "COOKIE_DESTINATION_URL cookie value",
            });
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Has.Exactly(1).Items);

            await Page.DeleteCookieAsync(new CookieParam { Name = cookieName });

            // Verify the cookie is deleted for the current page.
            Assert.That(await Page.GetCookiesAsync(), Is.Empty);

            // Verify the cookie is not deleted for different domain.
            var cookies = await Page.GetCookiesAsync(cookieDestinationUrl);
            Assert.That(cookies, Has.Exactly(1).Items);
            Assert.That(cookies[0].Name, Is.EqualTo(cookieName));
            Assert.That(cookies[0].Value, Is.EqualTo("COOKIE_DESTINATION_URL cookie value"));
            Assert.That(cookies[0].Domain, Is.EqualTo("example.com"));
            Assert.That(cookies[0].Path, Is.EqualTo("/"));
            Assert.That(cookies[0].Expires, Is.EqualTo(-1));
            Assert.That(cookies[0].Size, Is.EqualTo(51));
            Assert.That(cookies[0].HttpOnly, Is.False);
            Assert.That(cookies[0].Session, Is.True);
        }

        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should delete cookie for specified URL")]
        public async Task ShouldDeleteCookieForSpecifiedUrl()
        {
            const string cookieDestinationUrl = "https://example.com";
            const string cookieName = "some_cookie_name";

            await Page.GoToAsync(TestConstants.EmptyPage);

            // Set a cookie for the current page.
            await Page.SetCookieAsync(new CookieParam
            {
                Name = cookieName,
                Value = "some_cookie_value",
            });
            Assert.That(await Page.GetCookiesAsync(), Has.Exactly(1).Items);

            // Set a cookie for specified URL.
            await Page.SetCookieAsync(new CookieParam
            {
                Url = cookieDestinationUrl,
                Name = cookieName,
                Value = "another_cookie_value",
            });
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Has.Exactly(1).Items);

            // Delete the cookie for specified URL.
            await Page.DeleteCookieAsync(new CookieParam
            {
                Url = cookieDestinationUrl,
                Name = cookieName,
            });

            // Verify the cookie is deleted for specified URL.
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Is.Empty);

            // Verify the cookie is not deleted for the current page.
            var cookies = await Page.GetCookiesAsync();
            Assert.That(cookies, Has.Exactly(1).Items);
            Assert.That(cookies[0].Name, Is.EqualTo(cookieName));
            Assert.That(cookies[0].Value, Is.EqualTo("some_cookie_value"));
            Assert.That(cookies[0].Domain, Is.EqualTo("localhost"));
            Assert.That(cookies[0].Path, Is.EqualTo("/"));
            Assert.That(cookies[0].Expires, Is.EqualTo(-1));
            Assert.That(cookies[0].Size, Is.EqualTo(33));
            Assert.That(cookies[0].HttpOnly, Is.False);
            Assert.That(cookies[0].Secure, Is.False);
            Assert.That(cookies[0].Session, Is.True);
        }

        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should delete cookie for specified URL regardless of the current page")]
        public async Task ShouldDeleteCookieForSpecifiedUrlRegardlessOfTheCurrentPage()
        {
            const string cookieDestinationUrl = "https://example.com";
            const string cookieName = "some_cookie_name";
            var url1 = TestConstants.EmptyPage;
            var url2 = TestConstants.CrossProcessHttpPrefix + "/empty.html";

            await Page.GoToAsync(url1);
            // Set a cookie for the COOKIE_DESTINATION from URL_1.
            await Page.SetCookieAsync(new CookieParam
            {
                Url = cookieDestinationUrl,
                Name = cookieName,
                Value = "Cookie from URL_1",
            });
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Has.Exactly(1).Items);

            await Page.GoToAsync(url2);
            // Set a cookie for the COOKIE_DESTINATION from URL_2.
            await Page.SetCookieAsync(new CookieParam
            {
                Url = cookieDestinationUrl,
                Name = cookieName,
                Value = "Cookie from URL_2",
            });
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Has.Exactly(1).Items);

            // Delete the cookie for the COOKIE_DESTINATION from URL_2.
            await Page.DeleteCookieAsync(new CookieParam
            {
                Name = cookieName,
                Url = cookieDestinationUrl,
            });

            // Expect the cookie for the COOKIE_DESTINATION from URL_2 is deleted.
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Is.Empty);

            // Navigate back to the URL_1.
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Expect the cookie for the COOKIE_DESTINATION from URL_1 is deleted.
            Assert.That(await Page.GetCookiesAsync(cookieDestinationUrl), Is.Empty);
        }

        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should only delete cookie from the default partition if partitionkey is not specified")]
        public async Task ShouldOnlyDeleteCookieFromTheDefaultPartitionIfPartitionkeyIsNotSpecified()
        {
            var url = new Uri(TestConstants.EmptyPage);
            await Page.GoToAsync(url.ToString());
            await Page.SetCookieAsync(new CookieParam
            {
                Url = url.ToString(),
                Name = "partitionCookie",
                Value = "partition",
                Secure = true,
                PartitionKey = new CookiePartitionKey { SourceOrigin = url.GetLeftPart(UriPartial.Authority) },
            });
            Assert.That(await Page.GetCookiesAsync(), Has.Exactly(1).Items);
            await Page.DeleteCookieAsync(new CookieParam
            {
                Url = url.ToString(),
                Name = "partitionCookie",
            });
            Assert.That(await Page.GetCookiesAsync(), Is.Empty);
        }

        [Test, PuppeteerTest("cookies.spec", "Cookie specs Page.deleteCookie", "should delete cookie with partition key if partition key is specified")]
        public async Task ShouldDeleteCookieWithPartitionKeyIfPartitionKeyIsSpecified()
        {
            var url = new Uri(TestConstants.EmptyPage);
            await Page.GoToAsync(url.ToString());
            var origin = TestConstants.IsChrome
                ? url.GetComponents(UriComponents.Scheme | UriComponents.Host, UriFormat.UriEscaped)
                : url.GetLeftPart(UriPartial.Authority);
            await Page.SetCookieAsync(new CookieParam
            {
                Url = url.ToString(),
                Name = "partitionCookie",
                Value = "partition",
                Secure = true,
                PartitionKey = new CookiePartitionKey { SourceOrigin = origin },
            });
            Assert.That(await Page.GetCookiesAsync(), Has.Exactly(1).Items);
            await Page.DeleteCookieAsync(new CookieParam
            {
                Url = url.ToString(),
                Name = "partitionCookie",
                PartitionKey = new CookiePartitionKey { SourceOrigin = origin },
            });
            Assert.That(await Page.GetCookiesAsync(), Is.Empty);
        }
    }
}
