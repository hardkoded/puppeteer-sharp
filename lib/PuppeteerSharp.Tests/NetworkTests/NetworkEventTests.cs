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

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "Page.Events.Request")]
        public async Task PageEventsRequest()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.AreEqual(TestConstants.EmptyPage, requests[0].Url);
            Assert.AreEqual(ResourceType.Document, requests[0].ResourceType);
            Assert.AreEqual(HttpMethod.Get, requests[0].Method);
            Assert.NotNull(requests[0].Response);
            Assert.AreEqual(Page.MainFrame, requests[0].Frame);
            Assert.AreEqual(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestServedFromCache")]
        public async Task PageEventsRequestServedFromCache()
        {
            var cached = new List<string>();
            Page.RequestServedFromCache += (_, e) => cached.Add(e.Request.Url.Split('/').Last());
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            Assert.IsEmpty(cached);
            await Page.ReloadAsync();
            Assert.AreEqual(new[] { "one-style.css" }, cached);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "Page.Events.Response")]
        public async Task PageEventsResponse()
        {
            var responses = new List<IResponse>();
            Page.Response += (_, e) => responses.Add(e.Response);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(responses, Has.Exactly(1).Items);
            Assert.AreEqual(TestConstants.EmptyPage, responses[0].Url);
            Assert.AreEqual(HttpStatusCode.OK, responses[0].Status);
            Assert.False(responses[0].FromCache);
            Assert.False(responses[0].FromServiceWorker);
            Assert.NotNull(responses[0].Request);

            var remoteAddress = responses[0].RemoteAddress;
            // Either IPv6 or IPv4, depending on environment.
            Assert.True(remoteAddress.IP == "[::1]" || remoteAddress.IP == "127.0.0.1");
            Assert.AreEqual(TestConstants.Port, remoteAddress.Port);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestFailed")]
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
            StringAssert.Contains("one-style.css", failedRequests[0].Url);
            Assert.Null(failedRequests[0].Response);
            Assert.AreEqual(ResourceType.StyleSheet, failedRequests[0].ResourceType);

            if (TestConstants.IsChrome)
            {
                Assert.AreEqual("net::ERR_FAILED", failedRequests[0].FailureText);
            }
            else
            {
                Assert.AreEqual("NS_ERROR_FAILURE", failedRequests[0].FailureText);
            }

            Assert.NotNull(failedRequests[0].Frame);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "Page.Events.RequestFinished")]
        public async Task PageEventsRequestFinished()
        {
            var requests = new List<IRequest>();
            Page.RequestFinished += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.AreEqual(TestConstants.EmptyPage, requests[0].Url);
            Assert.NotNull(requests[0].Response);
            Assert.AreEqual(HttpMethod.Get, requests[0].Method);
            Assert.AreEqual(Page.MainFrame, requests[0].Frame);
            Assert.AreEqual(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "should fire events in proper order")]
        public async Task ShouldFireEventsInProperOrder()
        {
            var events = new List<string>();
            Page.Request += (_, _) => events.Add("request");
            Page.Response += (_, _) => events.Add("response");
            Page.RequestFinished += (_, _) => events.Add("requestfinished");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(new[] { "request", "response", "requestfinished" }, events.ToArray());
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Network Events", "should support redirects")]
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
            Assert.AreEqual(new[] {
                $"GET {FOO_URL}",
                $"302 {FOO_URL}",
                $"DONE {FOO_URL}",
                $"GET {TestConstants.EmptyPage}",
                $"200 {TestConstants.EmptyPage}",
                $"DONE {TestConstants.EmptyPage}"
            }, events.ToArray());

            // Check redirect chain
            var redirectChain = response.Request.RedirectChain;
            Assert.That(redirectChain, Has.Exactly(1).Items);
            StringAssert.Contains("/foo.html", redirectChain[0].Url);
            Assert.AreEqual(TestConstants.Port, redirectChain[0].Response.RemoteAddress.Port);
        }
    }
}
