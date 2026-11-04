using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestHeadersTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.headers", "should define Browser in user agent header")]
        public async Task ShouldDefineBrowserInUserAgentHeader()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);

            if (TestConstants.IsChrome)
            {
                Assert.That(response.Request.Headers["User-Agent"], Does.Contain("Chrome"));
            }
            else
            {
                Assert.That(response.Request.Headers["User-Agent"], Does.Contain("Firefox"));
            }
        }

        [Test, PuppeteerTest("network.spec", "network Request.headers", "should report all headers sent to the server on response.request()")]
        public async Task ShouldReportAllHeadersSentToTheServerOnResponseRequest()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();

            var serverRequestTask = Server.WaitForRequest("/empty.html", request => ToDictionary(request.Headers));
            var responseTask = Page.GoToAsync(TestConstants.EmptyPage);
            await Task.WhenAll(serverRequestTask, responseTask);

            Assert.That(ToLowerKeys(responseTask.Result.Request.Headers), Is.EquivalentTo(serverRequestTask.Result));
        }

        [Test, PuppeteerTest("network.spec", "network Request.headers", "should report all headers sent to the server on requestfailed")]
        public async Task ShouldReportAllHeadersSentToTheServerOnRequestFailed()
        {
            Server.SetRoute("/fail.html", context =>
            {
                context.Abort();
                return Task.CompletedTask;
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();

            var failedTaskSource = new TaskCompletionSource<IRequest>();
            Page.RequestFailed += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    failedTaskSource.TrySetResult(e.Request);
                }
            };

            var serverRequestTask = Server.WaitForRequest("/fail.html", request => ToDictionary(request.Headers));
            var gotoTask = Page.GoToAsync(TestConstants.ServerUrl + "/fail.html").ContinueWith(_ => { });
            await Task.WhenAll(serverRequestTask, failedTaskSource.Task, gotoTask);

            Assert.That(ToLowerKeys(failedTaskSource.Task.Result.Headers), Is.EquivalentTo(serverRequestTask.Result));
        }

        private static Dictionary<string, string> ToDictionary(IHeaderDictionary headers)
            => headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value.ToString());

        private static Dictionary<string, string> ToLowerKeys(Dictionary<string, string> headers)
            => headers.ToDictionary(h => h.Key.ToLowerInvariant(), h => h.Value);
    }
}
