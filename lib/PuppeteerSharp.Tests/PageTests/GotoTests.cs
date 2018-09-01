using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GotoTests : PuppeteerPageBaseTest
    {
        public GotoTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldNavigateToAboutBlank()
        {
            var response = await Page.GoToAsync(TestConstants.AboutBlank);
            Assert.Null(response);
        }

        [Fact]
        public async Task ShouldNavigateToEmptyPageWithDOMContentLoaded()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, waitUntil: new[]
            {
                WaitUntilNavigation.DOMContentLoaded
            });
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Null(response.SecurityDetails);
        }

        [Fact]
        public async Task ShouldFailWhenServerReturns204()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => Page.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("net::ERR_ABORTED", exception.Message);
        }

        [Fact]
        public async Task ShouldReturnResponseWhenPageChangesItsURLAfterLoad()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/historyapi.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldWorkWithSubframesReturn204()
        {
            Server.SetRoute("/frames/frame.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        }

        [Theory]
        [InlineData(WaitUntilNavigation.Networkidle0)]
        [InlineData(WaitUntilNavigation.Networkidle2)]
        public async Task ShouldNavigateToEmptyPage(WaitUntilNavigation waitUntil)
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { waitUntil } });
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadUrl()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync("asdfasdf"));
            Assert.Contains("Cannot navigate to invalid URL", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadSSL()
        {
            Page.Request += (sender, e) => Assert.NotNull(e.Request);
            Page.RequestFinished += (sender, e) => Assert.NotNull(e.Request);
            Page.RequestFailed += (sender, e) => Assert.NotNull(e.Request);

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));
            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));
            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenMainResourcesFailedToLoad()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync("http://localhost:44123/non-existing-url"));
            Assert.Contains("net::ERR_CONNECTION_REFUSED", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", context => Task.Delay(-1));

            var exception = await Assert.ThrowsAnyAsync<Exception>(async ()
                => await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { Timeout = 1 }));
            Assert.Contains("Timeout Exceeded: 1ms", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", context => Task.Delay(-1));

            Page.DefaultNavigationTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("Timeout Exceeded: 1ms", exception.Message);
        }

        [Fact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            var loaded = false;
            void OnLoad(object sender, EventArgs e)
            {
                loaded = true;
                Page.Load -= OnLoad;
            }
            Page.Load += OnLoad;

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions { Timeout = 0, WaitUntil = new[] { WaitUntilNavigation.Load } });
            Assert.True(loaded);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            var response = await Page.GoToAsync("data:text/html,hello");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/not-found");
            Assert.Equal(HttpStatusCode.NotFound, response.Status);
        }

        [Fact]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/redirect/3.html");
            Server.SetRedirect("/redirect/3.html", TestConstants.EmptyPage);

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
        }

        [Fact]
        public async Task ShouldWaitForNetworkIdleToSucceedNavigation()
        {
            var responses = new List<TaskCompletionSource<Func<HttpResponse, Task>>>();
            var fetches = new Dictionary<string, TaskCompletionSource<bool>>();
            foreach (var url in new[] {
                "/fetch-request-a.js",
                "/fetch-request-b.js",
                "/fetch-request-c.js",
                "/fetch-request-d.js" })
            {
                fetches[url] = new TaskCompletionSource<bool>();
                Server.SetRoute(url, async context =>
                {
                    var taskCompletion = new TaskCompletionSource<Func<HttpResponse, Task>>();
                    responses.Add(taskCompletion);
                    fetches[context.Request.Path].SetResult(true);
                    var actionResponse = await taskCompletion.Task;
                    await actionResponse(context.Response);
                });
            }

            var initialFetchResourcesRequested = Task.WhenAll(
                Server.WaitForRequest("/fetch-request-a.js"),
                Server.WaitForRequest("/fetch-request-b.js"),
                Server.WaitForRequest("/fetch-request-c.js")
            );
            var secondFetchResourceRequested = Server.WaitForRequest("/fetch-request-d.js");

            var navigationFinished = false;
            var navigationTask = Page.GoToAsync(TestConstants.ServerUrl + "/networkidle.html",
                new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } })
                .ContinueWith(res =>
                {
                    navigationFinished = true;
                    return res.Result;
                });

            var pageLoaded = new TaskCompletionSource<bool>();
            void WaitPageLoad(object sender, EventArgs e)
            {
                pageLoaded.SetResult(true);
                Page.Load -= WaitPageLoad;
            }
            Page.Load += WaitPageLoad;
            await pageLoaded.Task;

            Assert.False(navigationFinished);

            await initialFetchResourcesRequested;

            Assert.False(navigationFinished);

            await Task.WhenAll(
                fetches["/fetch-request-a.js"].Task,
                fetches["/fetch-request-b.js"].Task,
                fetches["/fetch-request-c.js"].Task);

            foreach (var actionResponse in responses)
            {
                actionResponse.SetResult(response =>
                {
                    response.StatusCode = 404;
                    return response.WriteAsync("File not found");
                });
            }

            responses.Clear();

            await secondFetchResourceRequested;

            Assert.False(navigationFinished);

            await fetches["/fetch-request-d.js"].Task;

            foreach (var actionResponse in responses)
            {
                actionResponse.SetResult(response =>
                {
                    response.StatusCode = 404;
                    return response.WriteAsync("File not found");
                });
            }

            var navigationResponse = await navigationTask;
            Assert.Equal(HttpStatusCode.OK, navigationResponse.Status);
        }

        [Fact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) => requests.Add(e.Request);
            var dataUrl = "data:text/html,<div>yo</div>";
            var response = await Page.GoToAsync(dataUrl);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Single(requests);
            Assert.Equal(dataUrl, requests[0].Url);
        }

        [Fact]
        public async Task ShouldNavigateToURLWithHashAndFireRequestsWithoutHash()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) => requests.Add(e.Request);
            var response = await Page.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
        }

        [Fact]
        public async Task ShouldWorkWithSelfRequestingPage()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/self-request.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("self-request.html", response.Url);
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingAndShowTheUrlAtTheErrorMessage()
        {
            var url = TestConstants.HttpsPrefix + "/redirect/1.html";
            var exception = await Assert.ThrowsAnyAsync<NavigationException>(async () => await Page.GoToAsync(url));
            Assert.Contains(url, exception.Message);
            Assert.Contains(url, exception.Url);
        }

        [Fact]
        public async Task ResponseOkShouldBeTrueForFile()
        {
            var fileToNavigate = Path.Combine(Directory.GetCurrentDirectory(), Path.Combine("assets", "file-to-upload.txt"));
            var url = new Uri(fileToNavigate).AbsoluteUri;

            var response = await Page.GoToAsync(url);
            Assert.True(response.Ok);
        }
    }
}