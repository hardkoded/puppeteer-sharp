using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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
        public async Task PageEventsRequestShouldReportPostData()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", context => Task.CompletedTask);
            Request request = null;
            Page.Request += (sender, e) => request = e.Request;
            await Page.EvaluateExpressionHandleAsync("fetch('./post', { method: 'POST', body: JSON.stringify({ foo: 'bar'})})");
            Assert.NotNull(request);
            Assert.Equal("{\"foo\":\"bar\"}", request.PostData);
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
            Assert.NotNull(responses[0].Request);
        }

        [Fact]
        public async Task PageEventsResponseShouldProvideBody()
        {
            Response response = null;
            Page.Response += (sender, e) => response = e.Response;
            await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.NotNull(response);
            var responseText = await new HttpClient().GetStringAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.Equal(responseText, await response.TextAsync());
            Assert.Equal(JObject.Parse(responseText), await response.JsonAsync());
        }

        [Fact]
        public async Task PageEventsResponseShouldNotReportBodyUnlessRequestIsFinished()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Setup server to trap request.
            var serverResponseCompletion = new TaskCompletionSource<bool>();
            HttpResponse serverResponse = null;
            Server.SetRoute("/get", context =>
            {
                serverResponse = context.Response;
                context.Response.WriteAsync("hello ");
                return serverResponseCompletion.Task;
            });
            // Setup page to trap response.
            Response pageResponse = null;
            var requestFinished = false;
            Page.Response += (sender, e) => pageResponse = e.Response;
            Page.RequestFinished += (sender, e) => requestFinished = true;
            // send request and wait for server response
            Task WaitForPageResponseEvent()
            {
                var completion = new TaskCompletionSource<bool>();
                Page.Response += (sender, e) => completion.SetResult(true);
                return completion.Task;
            }
            await Task.WhenAll(
                Page.EvaluateExpressionAsync("fetch('/get', { method: 'GET'})"),
                WaitForPageResponseEvent()
            );

            Assert.NotNull(serverResponse);
            Assert.NotNull(pageResponse);
            Assert.Equal(HttpStatusCode.OK, pageResponse.Status);
            Assert.False(requestFinished);

            var responseText = pageResponse.TextAsync();
            // Write part of the response and wait for it to be flushed.
            await serverResponse.WriteAsync("wor");
            // Finish response.
            await serverResponse.WriteAsync("ld!");
            serverResponseCompletion.SetResult(true);
            Assert.Equal("hello world!", await responseText);
        }

        [Fact]
        public async Task PageEventsRequestFailed()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                if (e.Request.Url.EndsWith("css"))
                    await e.Request.AbortAsync();
                else
                    await e.Request.ContinueAsync();
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
            await Page.GoToAsync(FOO_URL);
            Assert.Equal(new[] {
                $"GET {FOO_URL}",
                $"302 {FOO_URL}",
                $"DONE {FOO_URL}",
                $"GET {TestConstants.EmptyPage}",
                $"200 {TestConstants.EmptyPage}",
                $"DONE {TestConstants.EmptyPage}"
            }, events);
        }
    }
}
