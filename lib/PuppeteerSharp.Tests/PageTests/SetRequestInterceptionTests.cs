using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetRequestInterceptionTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldIntercept()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Contains("empty.html", e.Request.Url);
                Assert.NotNull(e.Request.Headers);
                Assert.Equal(HttpMethod.Get, e.Request.Method);
                Assert.Null(e.Request.PostData);
                Assert.Equal(ResourceType.Document, e.Request.ResourceType);
                Assert.Equal(Page.MainFrame, e.Request.Frame);
                Assert.Equal("about:blank", e.Request.Frame.Url);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldStopIntercepting()
        {
            await Page.SetRequestInterceptionAsync(true);
            async void EventHandler(object sender, RequestEventArgs e)
            {
                await e.Request.ContinueAsync();
                Page.RequestCreated -= EventHandler;
            }
            Page.RequestCreated += EventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Fact]
        public async Task ShouldShowCustomHTTPHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["foo"] = "bar"
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Equal("bar", e.Request.Headers["foo"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldWorksWithCustomizingRefererHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = TestConstants.EmptyPage
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Equal(TestConstants.EmptyPage, e.Request.Headers["referer"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldBeAbortable()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                if (e.Request.Url.EndsWith(".css"))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
            var failedRequests = 0;
            Page.RequestFailed += (sender, e) => failedRequests++;
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.True(response.Ok);
            Assert.Null(response.Request.Failure);
            Assert.Equal(1, failedRequests);
        }

        [Fact]
        public async Task ShouldBeAbortableWithCustomErrorCodes()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                await e.Request.AbortAsync(RequestAbortErrorCode.InternetDisconnected);
            };
            Request failedRequest = null;
            Page.RequestFailed += (sender, e) => failedRequest = e.Request;
            await Page.GoToAsync(TestConstants.EmptyPage).ContinueWith(e => { });
            Assert.NotNull(failedRequest);
            Assert.Equal("net::ERR_INTERNET_DISCONNECTED", failedRequest.Failure);
        }

        [Fact]
        public async Task ShouldAmendHTTPHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                var headers = new Dictionary<string, object>(e.Request.Headers);
                headers["FOO"] = "bar";
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            var requestTask = Server.WaitForRequest("/sleep.zzz", request => request.Headers["foo"].ToString());
            await Task.WhenAll(
                requestTask,
                Page.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );
            Assert.Equal("bar", requestTask.Result);
        }

        [Fact]
        public async Task ShouldFailNavigationWhenAbortingMainResource()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) => await e.Request.AbortAsync();
            var exception = await Assert.ThrowsAsync<NavigationException>(
                () => Page.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("net::ERR_FAILED", exception.Message);
        }

        [Fact]
        public async Task ShouldWorkWithRedirects()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            Page.RequestCreated += async (sender, e) =>
            {
                await e.Request.ContinueAsync();
                requests.Add(e.Request);
            };

            Server.SetRedirect("/non-existing-page.html", "/non-existing-page-2.html");
            Server.SetRedirect("/non-existing-page-2.html", "/non-existing-page-3.html");
            Server.SetRedirect("/non-existing-page-3.html", "/non-existing-page-4.html");
            Server.SetRedirect("/non-existing-page-4.html", "/empty.html");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/non-existing-page.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("empty.html", response.Url);
            Assert.Equal(5, requests.Count);
            Assert.Equal(ResourceType.Document, requests[2].ResourceType);
        }

        [Fact]
        public async Task ShouldBeAbleToAbortRedirects()
        {
            await Page.SetRequestInterceptionAsync(true);
            Server.SetRedirect("/non-existing.json", "/non-existing-2.json");
            Server.SetRedirect("/non-existing-2.json", "/simple.html");
            Page.RequestCreated += async (sender, e) =>
            {
                if (e.Request.Url.Contains("non-existing-2"))
                    await e.Request.AbortAsync();
                else
                    await e.Request.ContinueAsync();
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            var result = await Page.EvaluateFunctionAsync<string>(@"async () => {
                try
                {
                    await fetch('/non-existing.json');
                }
                catch (e)
                {
                    return e.message;
                }
            }");
            Assert.Contains("Failed to fetch", result);
        }

        [Fact]
        public async Task ShouldWorkWithEqualRequests()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var responseCount = 1;
            Server.SetRoute("/zzz", context => context.Response.WriteAsync((responseCount++) * 11 + string.Empty));
            await Page.SetRequestInterceptionAsync(true);

            var spinner = false;
            // Cancel 2nd request.
            Page.RequestCreated += async (sender, e) =>
            {
                if (spinner)
                {
                    spinner = !spinner;
                    await e.Request.AbortAsync();
                }
                else
                {
                    spinner = !spinner;
                    await e.Request.ContinueAsync();
                }
            };
            var results = await Page.EvaluateExpressionAsync<string[]>(@"Promise.all([
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
            ])");
            Assert.Equal(new[] { "11", "FAILED", "22" }, results);
        }

        [Fact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {

        }

        [Fact]
        public async Task ShouldAbortDataServer()
        {

        }

        [Fact]
        public async Task ShouldNavigateToURLWithHashAndAndFireRequestsWithoutHash()
        {

        }

        [Fact]
        public async Task ShouldWorkWithEncodedServer()
        {

        }

        [Fact]
        public async Task ShouldWorkWithBadlyEncodedServer()
        {

        }

        [Fact]
        public async Task ShouldWorkWithEncodedServerNegative2()
        {

        }

        [Fact]
        public async Task ShouldNotThrowInvalidInterceptionIdIfTheRequestWasCancelled()
        {

        }

        [Fact]
        public async Task ShouldThrowIfInterceptionIsNotEnabled()
        {

        }
    }
}
