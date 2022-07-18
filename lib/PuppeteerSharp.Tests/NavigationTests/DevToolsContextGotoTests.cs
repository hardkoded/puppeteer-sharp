using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.DevTools.Dom.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.DevTools.Dom;
using Microsoft.AspNetCore.Http;

namespace PuppeteerSharp.Tests.NavigationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextGotoTests : DevToolsContextBaseTest
    {
        public DevToolsContextGotoTests(ITestOutputHelper output) : base(output, ignoreHTTPSerrors:false)
        {
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work with anchor navigation")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAnchorNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, DevToolsContext.Url);
            await DevToolsContext.GoToAsync($"{TestConstants.EmptyPage}#foo");
            Assert.Equal($"{TestConstants.EmptyPage}#foo", DevToolsContext.Url);
            await DevToolsContext.GoToAsync($"{TestConstants.EmptyPage}#bar");
            Assert.Equal($"{TestConstants.EmptyPage}#bar", DevToolsContext.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work with redirects")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRedirects()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/empty.html");

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to about:blank")]
        [PuppeteerFact]
        public async Task ShouldNavigateToAboutBlank()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.AboutBlank);
            Assert.Null(response);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should return response when page changes its URL after load")]
        [PuppeteerFact]
        public async Task ShouldReturnResponseWhenPageChangesItsURLAfterLoad()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/historyapi.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work with subframes return 204")]
        [PuppeteerFact]
        public async Task ShouldWorkWithSubframesReturn204()
        {
            Server.SetRoute("/frames/frame.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when server returns 204")]
        [PuppeteerFact]
        public async Task ShouldFailWhenServerReturns204()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => DevToolsContext.GoToAsync(TestConstants.EmptyPage));

            Assert.Contains("net::ERR_ABORTED", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to empty page with domcontentloaded")]
        [PuppeteerFact]
        public async Task ShouldNavigateToEmptyPageWithDOMContentLoaded()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage, waitUntil: new[]
            {
                WaitUntilNavigation.DOMContentLoaded
            });
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Null(response.SecurityDetails);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work when page calls history API in beforeunload")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenPageCallsHistoryAPIInBeforeunload()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync(@"() =>
            {
                window.addEventListener('beforeunload', () => history.replaceState(null, 'initial', window.location.href), false);
            }");
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to empty page with networkidle0")]
        [PuppeteerFact]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle0()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } });
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to empty page with networkidle2")]
        [PuppeteerFact]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle2()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when navigating to bad url")]
        [PuppeteerFact]
        public async Task ShouldFailWhenNavigatingToBadUrl()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync("asdfasdf"));

            Assert.Contains("Cannot navigate to invalid URL", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when navigating to bad SSL")]
        [PuppeteerFact]
        public async Task ShouldFailWhenNavigatingToBadSSL()
        {
            DevToolsContext.Request += (_, e) => Assert.NotNull(e.Request);
            DevToolsContext.RequestFinished += (_, e) => Assert.NotNull(e.Request);
            DevToolsContext.RequestFailed += (_, e) => Assert.NotNull(e.Request);

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));

            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when navigating to bad SSL after redirects")]
        [PuppeteerFact]
        public async Task ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));

            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when main resources failed to load")]
        [PuppeteerFact]
        public async Task ShouldFailWhenMainResourcesFailedToLoad()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync("http://localhost:44123/non-existing-url"));

            Assert.Contains("net::ERR_CONNECTION_REFUSED", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when exceeding maximum navigation timeout")]
        [PuppeteerFact]
        public async Task ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            var exception = await Assert.ThrowsAnyAsync<Exception>(async ()
                => await DevToolsContext.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { Timeout = 1 }));
            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when exceeding default maximum navigation timeout")]
        [PuppeteerFact]
        public async Task ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            DevToolsContext.DefaultNavigationTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when exceeding default maximum timeout")]
        [PuppeteerFact]
        public async Task ShouldFailWhenExceedingDefaultMaximumTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            DevToolsContext.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should prioritize default navigation timeout over default timeout")]
        [PuppeteerFact]
        public async Task ShouldPrioritizeDefaultNavigationTimeoutOverDefaultTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            DevToolsContext.DefaultTimeout = 0;
            DevToolsContext.DefaultNavigationTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.GoToAsync(TestConstants.EmptyPage));
            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should disable timeout when its set to 0")]
        [PuppeteerFact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            var loaded = false;
            void OnLoad(object sender, EventArgs e)
            {
                loaded = true;
                DevToolsContext.Load -= OnLoad;
            }
            DevToolsContext.Load += OnLoad;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions { Timeout = 0, WaitUntil = new[] { WaitUntilNavigation.Load } });
            Assert.True(loaded);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work when navigating to valid url")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work when navigating to data url")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            var response = await DevToolsContext.GoToAsync("data:text/html,hello");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work when navigating to 404")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/not-found");
            Assert.Equal(HttpStatusCode.NotFound, response.Status);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should return last response in redirect chain")]
        [PuppeteerFact]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/redirect/3.html");
            Server.SetRedirect("/redirect/3.html", TestConstants.EmptyPage);

            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should wait for network idle to succeed navigation")]
        [PuppeteerFact]
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
                    await actionResponse(context.Response).WithTimeout();
                });
            }

            var initialFetchResourcesRequested = Task.WhenAll(
                Server.WaitForRequest("/fetch-request-a.js"),
                Server.WaitForRequest("/fetch-request-b.js"),
                Server.WaitForRequest("/fetch-request-c.js")
            );
            var secondFetchResourceRequested = Server.WaitForRequest("/fetch-request-d.js");

            var pageLoaded = new TaskCompletionSource<bool>();
            void WaitPageLoad(object sender, EventArgs e)
            {
                pageLoaded.SetResult(true);
                DevToolsContext.Load -= WaitPageLoad;
            }
            DevToolsContext.Load += WaitPageLoad;

            var navigationFinished = false;
            var navigationTask = DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/networkidle.html",
                new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle0 } })
                .ContinueWith(res =>
                {
                    navigationFinished = true;
                    return res.Result;
                });

            await pageLoaded.Task.WithTimeout();

            Assert.False(navigationFinished);

            await initialFetchResourcesRequested.WithTimeout();

            Assert.False(navigationFinished);

            await Task.WhenAll(
                fetches["/fetch-request-a.js"].Task,
                fetches["/fetch-request-b.js"].Task,
                fetches["/fetch-request-c.js"].Task).WithTimeout();

            foreach (var actionResponse in responses)
            {
                actionResponse.SetResult(response =>
                {
                    response.StatusCode = 404;
                    return response.WriteAsync("File not found");
                });
            }

            responses.Clear();

            await secondFetchResourceRequested.WithTimeout();

            Assert.False(navigationFinished);

            await fetches["/fetch-request-d.js"].Task.WithTimeout();

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

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to dataURL and fire dataURL requests")]
        [PuppeteerFact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            var dataUrl = "data:text/html,<div>yo</div>";
            var response = await DevToolsContext.GoToAsync(dataUrl);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Single(requests);
            Assert.Equal(dataUrl, requests[0].Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should navigate to URL with hash and fire requests without hash")]
        [PuppeteerFact]
        public async Task ShouldNavigateToURLWithHashAndFireRequestsWithoutHash()
        {
            var requests = new List<Request>();
            DevToolsContext.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should work with self requesting page")]
        [PuppeteerFact]
        public async Task ShouldWorkWithSelfRequestingPage()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/self-request.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("self-request.html", response.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should fail when navigating and show the url at the error message")]
        [PuppeteerFact]
        public async Task ShouldFailWhenNavigatingAndShowTheUrlAtTheErrorMessage()
        {
            var url = TestConstants.HttpsPrefix + "/redirect/1.html";
            var exception = await Assert.ThrowsAnyAsync<NavigationException>(async () => await DevToolsContext.GoToAsync(url));
            Assert.Contains(url, exception.Message);
            Assert.Contains(url, exception.Url);
        }

        [PuppeteerTest("navigation.spec.ts", "Page.goto", "should send referer")]
        [PuppeteerFact]
        public async Task ShouldSendReferer()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            string referer1 = null;
            string referer2 = null;

            await Task.WhenAll(
                Server.WaitForRequest("/grid.html", r => referer1 = r.Headers["Referer"]),
                Server.WaitForRequest("/digits/1.png", r => referer2 = r.Headers["Referer"]),
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions
                {
                    Referer = "http://google.com/"
                })
            );

            Assert.Equal("http://google.com/", referer1);
            // Make sure subresources do not inherit referer.
            Assert.Equal(TestConstants.ServerUrl + "/grid.html", referer2);
        }
    }
}
