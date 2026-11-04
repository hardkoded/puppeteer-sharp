using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RawNetworkHeadersTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network raw network headers", "Same-origin set-cookie navigation")]
        public async Task SameOriginSetCookieNavigation()
        {
            var setCookieString = "foo=bar";
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.Headers["set-cookie"] = setCookieString;
                return context.Response.WriteAsync("hello world");
            });
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Headers["set-cookie"], Is.EqualTo(setCookieString));
        }

        [Test, PuppeteerTest("network.spec", "network raw network headers", "Same-origin set-cookie subresource")]
        public async Task SameOriginSetCookieSubresource()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var setCookieString = "foo=bar";
            Server.SetRoute("/foo", context =>
            {
                context.Response.Headers["set-cookie"] = setCookieString;
                return context.Response.WriteAsync("hello world");
            });

            var responseTask = Page.WaitForResponseAsync(res =>
                !TestUtils.IsFavicon(res.Request));

            await Page.EvaluateExpressionAsync(@"
                const xhr = new XMLHttpRequest();
                xhr.open('GET', '/foo');
                xhr.send();
            ");

            var response = await responseTask;
            Assert.That(response.Headers["set-cookie"], Is.EqualTo(setCookieString));
        }

        [Test, PuppeteerTest("network.spec", "network raw network headers", "Cross-origin set-cookie")]
        public async Task CrossOriginSetCookie()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            var setCookieString = "hello=world";
            Server.SetRoute("/setcookie.html", context =>
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["set-cookie"] = setCookieString;
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/setcookie.html");
            var url = TestConstants.CrossProcessHttpPrefix + "/setcookie.html";
            var responseTask = Page.WaitForResponseAsync(response => response.Url == url);

            await Page.EvaluateFunctionAsync(@"(src) => {
                const xhr = new XMLHttpRequest();
                xhr.open('GET', src);
                xhr.send();
            }", url);

            var response = await responseTask;
            Assert.That(response.Headers["set-cookie"], Is.EqualTo(setCookieString));
        }
    }
}
