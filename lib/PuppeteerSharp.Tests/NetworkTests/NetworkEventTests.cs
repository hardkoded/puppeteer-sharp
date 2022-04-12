using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class NetworkEventTests : PuppeteerPageBaseTest
    {
        public NetworkEventTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "Page.Events.Request")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageEventsRequest()
        {
            var requests = new List<Request>();
            Page.Request += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.Equal(ResourceType.Document, requests[0].ResourceType);
            Assert.Equal(HttpMethod.Get, requests[0].Method);
            Assert.NotNull(requests[0].Response);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "Page.Events.RequestServedFromCache")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageEventsRequestServedFromCache()
        {
            var cached= new List<string>();
            Page.RequestServedFromCache += (_, e) => cached.Add(e.Request.Url.Split('/').Last());
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            Assert.Empty(cached);
            await Page.ReloadAsync();
            Assert.Equal(new[] { "one-style.css" }, cached);
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "Page.Events.Response")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageEventsResponse()
        {
            var responses = new List<Response>();
            Page.Response += (_, e) => responses.Add(e.Response);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(responses);
            Assert.Equal(TestConstants.EmptyPage, responses[0].Url);
            Assert.Equal(HttpStatusCode.OK, responses[0].Status);
            Assert.False(responses[0].FromCache);
            Assert.False(responses[0].FromServiceWorker);
            Assert.NotNull(responses[0].Request);

            var remoteAddress = responses[0].RemoteAddress;
            // Either IPv6 or IPv4, depending on environment.
            Assert.True(remoteAddress.IP == "[::1]" || remoteAddress.IP == "127.0.0.1");
            Assert.Equal(TestConstants.Port, remoteAddress.Port);
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "Page.Events.RequestFailed")]
        [SkipBrowserFact(skipFirefox: true)]
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
            var failedRequests = new List<Request>();
            Page.RequestFailed += (_, e) => failedRequests.Add(e.Request);
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");

            Assert.Single(failedRequests);
            Assert.Contains("one-style.css", failedRequests[0].Url);
            Assert.Null(failedRequests[0].Response);
            Assert.Equal(ResourceType.StyleSheet, failedRequests[0].ResourceType);

            if (TestConstants.IsChrome)
            {
                Assert.Equal("net::ERR_FAILED", failedRequests[0].Failure);
            }
            else
            {
                Assert.Equal("NS_ERROR_FAILURE", failedRequests[0].Failure);
            }

            Assert.NotNull(failedRequests[0].Frame);
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "Page.Events.RequestFinished")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task PageEventsRequestFinished()
        {
            var requests = new List<Request>();
            Page.RequestFinished += (_, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.NotNull(requests[0].Response);
            Assert.Equal(HttpMethod.Get, requests[0].Method);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "should fire events in proper order")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFireEventsInProperOrder()
        {
            var events = new List<string>();
            Page.Request += (_, _) => events.Add("request");
            Page.Response += (_, _) => events.Add("response");
            Page.RequestFinished += (_, _) => events.Add("requestfinished");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(new[] { "request", "response", "requestfinished" }, events.ToArray());
        }

        [PuppeteerTest("network.spec.ts", "Network Events", "should support redirects")]
        [SkipBrowserFact(skipFirefox: true)]
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
            Assert.Equal(new[] {
                $"GET {FOO_URL}",
                $"302 {FOO_URL}",
                $"DONE {FOO_URL}",
                $"GET {TestConstants.EmptyPage}",
                $"200 {TestConstants.EmptyPage}",
                $"DONE {TestConstants.EmptyPage}"
            }, events.ToArray());

            // Check redirect chain
            var redirectChain = response.Request.RedirectChain;
            Assert.Single(redirectChain);
            Assert.Contains("/foo.html", redirectChain[0].Url);
            Assert.Equal(TestConstants.Port, redirectChain[0].Response.RemoteAddress.Port);
        }
    }
}
