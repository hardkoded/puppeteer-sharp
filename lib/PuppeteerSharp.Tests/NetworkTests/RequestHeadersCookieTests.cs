using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestHeadersCookieTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.headers cookie header", "should show Cookie header")]
        public async Task ShouldShowCookieHeader()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");

            Assert.That(response.Request.Headers.TryGetValue("cookie", out var cookie), Is.True, "Cookie header should be present");
            Assert.That(cookie, Does.Contain("username=John Doe"));
        }

        [Test, PuppeteerTest("network.spec", "network Request.headers cookie header", "should show Cookie header for redirect")]
        public async Task ShouldShowCookieHeaderForRedirect()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRedirect("/foo.html", "/title.html");
            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/foo.html");

            Assert.That(response.Request.RedirectChain[0].Headers.TryGetValue("cookie", out var cookie1), Is.True, "Cookie header should be present in redirect chain");
            Assert.That(cookie1, Does.Contain("username=John Doe"));

            Assert.That(response.Request.Headers.TryGetValue("cookie", out var cookie2), Is.True, "Cookie header should be present in final request");
            Assert.That(cookie2, Does.Contain("username=John Doe"));
        }

        [Test, PuppeteerTest("network.spec", "network Request.headers cookie header", "should show Cookie header for fetch request")]
        public async Task ShouldShowCookieHeaderForFetchRequest()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("document.cookie = 'username=John Doe'");

            var responseTask = Page.WaitForResponseAsync(response => !TestUtils.IsFavicon(response.Request));
            await Page.EvaluateExpressionAsync("fetch('/title.html')");
            var response = await responseTask.WithTimeout();

            Assert.That(response.Request.Headers.TryGetValue("cookie", out var cookie), Is.True, "Cookie header should be present");
            Assert.That(cookie, Does.Contain("username=John Doe"));
        }
    }
}
