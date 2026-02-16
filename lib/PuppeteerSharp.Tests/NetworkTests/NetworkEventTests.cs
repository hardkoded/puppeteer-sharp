using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class NetworkEventTests : PuppeteerPageBaseTest
    {
        public NetworkEventTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.Request")]
        public async Task PageEventsRequest()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.That(requests[0].Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(requests[0].ResourceType, Is.EqualTo(ResourceType.Document));
            Assert.That(requests[0].Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(requests[0].Response, Is.Not.Null);
            Assert.That(requests[0].Frame, Is.EqualTo(Page.MainFrame));
            Assert.That(requests[0].Frame.Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestServedFromCache")]
        public async Task PageEventsRequestServedFromCache()
        {
            // Use a fresh browser context to avoid shared HTTP cache from prior tests.
            await using var context = await Browser.CreateBrowserContextAsync();
            var page = await context.NewPageAsync();

            var cached = new List<string>();
            page.RequestServedFromCache += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    cached.Add(e.Request.Url.Split('/').Last());
                }
            };
            await page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            Assert.That(cached, Is.Empty);
            await Task.Delay(1000);
            await page.ReloadAsync();
            Assert.That(cached, Is.EqualTo(new[] { "one-style.css" }));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.Response")]
        public async Task PageEventsResponse()
        {
            var responses = new List<IResponse>();
            Page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    responses.Add(e.Response);
                }
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(responses, Has.Exactly(1).Items);
            Assert.That(responses[0].Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(responses[0].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses[0].FromCache, Is.False);
            Assert.That(responses[0].FromServiceWorker, Is.False);
            Assert.That(responses[0].Request, Is.Not.Null);
        }

        [Test, PuppeteerTest("network.spec", "network Response.remoteAddress", "should work")]
        public async Task ResponseRemoteAddressShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            var remoteAddress = response.RemoteAddress;
            // Either IPv6 or IPv4, depending on environment.
            Assert.That(remoteAddress.IP == "[::1]" || remoteAddress.IP == "127.0.0.1", Is.True);
            Assert.That(remoteAddress.Port, Is.EqualTo(TestConstants.Port));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestFailed")]
        public async Task PageEventsRequestFailed()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                if (e.Request.Url.EndsWith("css"))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
            var failedRequests = new List<IRequest>();
            Page.RequestFailed += (_, e) => failedRequests.Add(e.Request);
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");

            Assert.That(failedRequests, Has.Exactly(1).Items);
            Assert.That(failedRequests[0].Url, Does.Contain("one-style.css"));
            Assert.That(failedRequests[0].Response, Is.Null);
            Assert.That(failedRequests[0].ResourceType, Is.EqualTo(ResourceType.StyleSheet));

            if (TestConstants.IsChrome)
            {
                Assert.That(failedRequests[0].FailureText, Is.EqualTo("net::ERR_FAILED"));
            }
            else
            {
                Assert.That(failedRequests[0].FailureText, Is.EqualTo("NS_ERROR_ABORT"));
            }

            Assert.That(failedRequests[0].Frame, Is.Not.Null);
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestFinished")]
        public async Task PageEventsRequestFinished()
        {
            var requests = new List<IRequest>();
            Page.RequestFinished += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.That(requests[0].Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(requests[0].Response, Is.Not.Null);
            Assert.That(requests[0].Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(requests[0].Frame, Is.EqualTo(Page.MainFrame));
            Assert.That(requests[0].Frame.Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "should fire events in proper order")]
        public async Task ShouldFireEventsInProperOrder()
        {
            var events = new List<string>();
            Page.Request += (_, _) => events.Add("request");
            Page.Response += (_, _) => events.Add("response");
            Page.RequestFinished += (_, _) => events.Add("requestfinished");
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Events can sneak in after the page has navigated (e.g., favicon requests)
            Assert.That(events.Take(3).ToArray(), Is.EqualTo(new[] { "request", "response", "requestfinished" }));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "should support redirects")]
        public async Task ShouldSupportRedirects()
        {
            var events = new List<string>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    events.Add($"{e.Request.Method} {e.Request.Url}");
                }
            };
            Page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    events.Add($"{(int)e.Response.Status} {e.Response.Url}");
                }
            };
            Page.RequestFinished += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    events.Add($"DONE {e.Request.Url}");
                }
            };
            Page.RequestFailed += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    events.Add($"FAIL {e.Request.Url}");
                }
            };
            Server.SetRedirect("/foo.html", "/empty.html");
            var FOO_URL = TestConstants.ServerUrl + "/foo.html";
            var response = await Page.GoToAsync(FOO_URL);
            Assert.That(events.ToArray(), Is.EqualTo(new[] {
                $"GET {FOO_URL}",
                $"302 {FOO_URL}",
                $"DONE {FOO_URL}",
                $"GET {TestConstants.EmptyPage}",
                $"200 {TestConstants.EmptyPage}",
                $"DONE {TestConstants.EmptyPage}"
            }));

            // Check redirect chain
            var redirectChain = response.Request.RedirectChain;
            Assert.That(redirectChain, Has.Exactly(1).Items);
            Assert.That(redirectChain[0].Url, Does.Contain("/foo.html"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.remoteAddress", "should support redirects")]
        public async Task ResponseRemoteAddressShouldSupportRedirects()
        {
            Server.SetRedirect("/foo.html", "/empty.html");
            var FOO_URL = TestConstants.ServerUrl + "/foo.html";
            var response = await Page.GoToAsync(FOO_URL);

            // Check redirect chain remote address
            var redirectChain = response.Request.RedirectChain;
            Assert.That(redirectChain, Has.Exactly(1).Items);
            Assert.That(redirectChain[0].Url, Does.Contain("/foo.html"));
            Assert.That(redirectChain[0].Response.RemoteAddress.Port, Is.EqualTo(TestConstants.Port));
        }
    }
}
