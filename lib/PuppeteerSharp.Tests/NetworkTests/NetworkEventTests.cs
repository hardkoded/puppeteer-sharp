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
            Page.Request += (_, e) => requests.Add(e.Request);
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
            var cached = new List<string>();
            Page.RequestServedFromCache += (_, e) => cached.Add(e.Request.Url.Split('/').Last());
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            Assert.That(cached, Is.Empty);
            await Page.ReloadAsync();
            Assert.That(cached, Is.EqualTo(new[] { "one-style.css" }));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.Response")]
        public async Task PageEventsResponse()
        {
            var responses = new List<IResponse>();
            Page.Response += (_, e) => responses.Add(e.Response);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(responses, Has.Exactly(1).Items);
            Assert.That(responses[0].Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(responses[0].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses[0].FromCache, Is.False);
            Assert.That(responses[0].FromServiceWorker, Is.False);
            Assert.That(responses[0].Request, Is.Not.Null);

            var remoteAddress = responses[0].RemoteAddress;
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
                Assert.That(failedRequests[0].FailureText, Is.EqualTo("NS_ERROR_FAILURE"));
            }

            Assert.That(failedRequests[0].Frame, Is.Not.Null);
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestFinished")]
        public async Task PageEventsRequestFinished()
        {
            var requests = new List<IRequest>();
            Page.RequestFinished += (_, e) => requests.Add(e.Request);
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
            Assert.That(events.ToArray(), Is.EqualTo(new[] { "request", "response", "requestfinished" }));
        }

        [Test, PuppeteerTest("network.spec", "network Network Events", "should support redirects")]
        public async Task ShouldSupportRedirects()
        {
            var events = new List<string>();
            Page.Request += (_, e) => events.Add($"{e.Request.Method} {e.Request.Url}");
            Page.Response += (_, e) => events.Add($"{(int)e.Response.Status} {e.Response.Url}");
            Page.RequestFinished += (_, e) => events.Add($"DONE {e.Request.Url}");
            Page.RequestFailed += (_, e) => events.Add($"FAIL {e.Request.Url}");
            Server.SetRedirect("/foo.html", "/empty.html");
            const string FOO_URL = TestConstants.ServerUrl + "/foo.html";
            var response = await Page.GoToAsync(FOO_URL);
            System.Console.WriteLine(string.Concat(events, ','));
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
            Assert.That(redirectChain[0].Response.RemoteAddress.Port, Is.EqualTo(TestConstants.Port));
        }
    }
}
