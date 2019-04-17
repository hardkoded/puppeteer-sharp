using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class NetworkEventTests : PuppeteerPageBaseTest
    {
        public NetworkEventTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task PageEventsRequest()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.Equal(ResourceType.Document, requests[0].ResourceType);
            Assert.Equal(HttpMethod.Get, requests[0].Method);
            Assert.NotNull(requests[0].Response);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [Fact]
        public async Task PageEventsResponse()
        {
            var responses = new List<Response>();
            Page.Response += (sender, e) => responses.Add(e.Response);
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

        [Fact]
        public async Task PageEventsRequestFailed()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
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
            Page.RequestFailed += (sender, e) => failedRequests.Add(e.Request);
            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");

            Assert.Single(failedRequests);
            Assert.Contains("one-style.css", failedRequests[0].Url);
            Assert.Null(failedRequests[0].Response);
            Assert.Equal(ResourceType.StyleSheet, failedRequests[0].ResourceType);
            Assert.Equal("net::ERR_FAILED", failedRequests[0].Failure);
            Assert.NotNull(failedRequests[0].Frame);
        }

        [Fact]
        public async Task PageEventsRequestFinished()
        {
            var requests = new List<Request>();
            Page.RequestFinished += (sender, e) => requests.Add(e.Request);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.NotNull(requests[0].Response);
            Assert.Equal(HttpMethod.Get, requests[0].Method);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);
        }

        [Fact]
        public async Task ShouldFireEventsInProperOrder()
        {
            var events = new List<string>();
            Page.Request += (sender, e) => events.Add("request");
            Page.Response += (sender, e) => events.Add("response");
            Page.RequestFinished += (sender, e) => events.Add("requestfinished");
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(new[] { "request", "response", "requestfinished" }, events);
        }

        [Fact]
        public async Task ShouldSupportRedirects()
        {
            var events = new List<string>();
            Page.Request += (sender, e) => events.Add($"{e.Request.Method} {e.Request.Url}");
            Page.Response += (sender, e) => events.Add($"{(int)e.Response.Status} {e.Response.Url}");
            Page.RequestFinished += (sender, e) => events.Add($"DONE {e.Request.Url}");
            Page.RequestFailed += (sender, e) => events.Add($"FAIL {e.Request.Url}");
            Server.SetRedirect("/foo.html", "/empty.html");
            const string FOO_URL = TestConstants.ServerUrl + "/foo.html";
            var response = await Page.GoToAsync(FOO_URL);
            Assert.Equal(new[] {
                $"GET {FOO_URL}",
                $"302 {FOO_URL}",
                $"DONE {FOO_URL}",
                $"GET {TestConstants.EmptyPage}",
                $"200 {TestConstants.EmptyPage}",
                $"DONE {TestConstants.EmptyPage}"
            }, events);

            // Check redirect chain
            var redirectChain = response.Request.RedirectChain;
            Assert.Single(redirectChain);
            Assert.Contains("/foo.html", redirectChain[0].Url);
            Assert.Equal(TestConstants.Port, redirectChain[0].Response.RemoteAddress.Port);
        }
    }
}