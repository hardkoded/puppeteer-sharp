using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageGotoTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(TestConstants.EmptyPage, Page.Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with anchor navigation")]
        public async Task ShouldWorkWithAnchorNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(TestConstants.EmptyPage, Page.Url);
            await Page.GoToAsync($"{TestConstants.EmptyPage}#foo");
            Assert.AreEqual($"{TestConstants.EmptyPage}#foo", Page.Url);
            await Page.GoToAsync($"{TestConstants.EmptyPage}#bar");
            Assert.AreEqual($"{TestConstants.EmptyPage}#bar", Page.Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with redirects")]
        public async Task ShouldWorkWithRedirects()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/empty.html");

            await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to about:blank")]
        public async Task ShouldNavigateToAboutBlank()
        {
            var response = await Page.GoToAsync(TestConstants.AboutBlank);
            Assert.Null(response);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should return response when page changes its URL after load")]
        public async Task ShouldReturnResponseWhenPageChangesItsURLAfterLoad()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/historyapi.html");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with subframes return 204")]
        public async Task ShouldWorkWithSubframesReturn204()
        {
            Server.SetRoute("/frames/frame.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when server returns 204")]
        public void ShouldFailWhenServerReturns204()
        {
            Server.SetRoute("/empty.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            var exception = Assert.ThrowsAsync<NavigationException>(
                () => Page.GoToAsync(TestConstants.EmptyPage));

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("net::ERR_ABORTED", exception.Message);
            }
            else
            {
                StringAssert.Contains("NS_BINDING_ABORTED", exception.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with domcontentloaded")]
        public async Task ShouldNavigateToEmptyPageWithDOMContentLoaded()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, waitUntil: new[]
            {
                WaitUntilNavigation.DOMContentLoaded
            });
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.Null(response.SecurityDetails);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when page calls history API in beforeunload")]
        public async Task ShouldWorkWhenPageCallsHistoryAPIInBeforeunload()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() =>
            {
                window.addEventListener('beforeunload', () => history.replaceState(null, 'initial', window.location.href), false);
            }");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with networkidle0")]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle0()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions
            {
                WaitUntil =
                [WaitUntilNavigation.Networkidle0]
            });
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }


        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to page with iframe and networkidle0")]
        public async Task ShouldNavigateToPageWithIframeAndNetworkidle0()

        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html", new NavigationOptions
            {
                WaitUntil =
                [WaitUntilNavigation.Networkidle0]
            });
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with networkidle2")]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle2()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad url")]
        public void ShouldFailWhenNavigatingToBadUrl()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync("asdfasdf"));

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("Cannot navigate to invalid URL", exception.Message);
            }
            else
            {
                StringAssert.Contains("invalid URL", exception.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad SSL")]
        public void ShouldFailWhenNavigatingToBadSSL()
        {
            Page.Request += (_, e) => Assert.NotNull(e.Request);
            Page.RequestFinished += (_, e) => Assert.NotNull(e.Request);
            Page.RequestFailed += (_, e) => Assert.NotNull(e.Request);

            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
            }
            else
            {
                StringAssert.Contains("SSL_ERROR_UNKNOWN", exception.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad SSL after redirects")]
        public void ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
            }
            else
            {
                StringAssert.Contains("SSL_ERROR_UNKNOWN", exception.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when main resources failed to load")]
        public void ShouldFailWhenMainResourcesFailedToLoad()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync("http://localhost:44123/non-existing-url"));

            if (TestConstants.IsChrome)
            {
                StringAssert.Contains("net::ERR_CONNECTION_REFUSED", exception.Message);
            }
            else
            {
                StringAssert.Contains("NS_ERROR_CONNECTION_REFUSED", exception.Message);
            }
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding maximum navigation timeout")]
        public void ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            var exception = Assert.ThrowsAsync<NavigationException>(async ()
                => await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { Timeout = 1 }));
            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding default maximum navigation timeout")]
        public void ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            Page.DefaultNavigationTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding default maximum timeout")]
        public void ShouldFailWhenExceedingDefaultMaximumTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should prioritize default navigation timeout over default timeout")]
        public void ShouldPrioritizeDefaultNavigationTimeoutOverDefaultTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            Page.DefaultTimeout = 0;
            Page.DefaultNavigationTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should disable timeout when its set to 0")]
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

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to valid url")]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to data url")]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            var response = await Page.GoToAsync("data:text/html,hello");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to 404")]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/not-found");
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should not throw an error for a 404 response with an empty body")]
        public async Task ShouldNotThrowAnErrorForA404ResponseWithAnEmptyBody()
        {
            Server.SetRoute("/404-error", context =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/404-error");
            Assert.False(response.Ok);
            Assert.AreEqual(HttpStatusCode.NotFound, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should not throw an error for a 500 response with an empty body")]
        public async Task ShouldNotThrowAnErrorForA500ResponseWithAnEmptyBody()
        {
            Server.SetRoute("/500-error", context =>
            {
                context.Response.StatusCode = 500;
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/500-error");
            Assert.False(response.Ok);
            Assert.AreEqual(HttpStatusCode.InternalServerError, response.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should return last response in redirect chain")]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/redirect/3.html");
            Server.SetRedirect("/redirect/3.html", TestConstants.EmptyPage);

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreEqual(TestConstants.EmptyPage, response.Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should wait for network idle to succeed navigation")]
        public async Task ShouldWaitForNetworkIdleToSucceedNavigation()
        {
            var responses = new ConcurrentSet<TaskCompletionSource<Func<HttpResponse, Task>>>();
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
                Page.Load -= WaitPageLoad;
            }
            Page.Load += WaitPageLoad;

            var navigationFinished = false;
            var navigationTask = Page.GoToAsync(TestConstants.ServerUrl + "/networkidle.html",
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

            var initialResponses = responses.ToArray();
            responses.Clear();

            foreach (var actionResponse in initialResponses)
            {
                actionResponse.SetResult(response =>
                {
                    response.StatusCode = 404;
                    return response.WriteAsync("File not found");
                });
            }

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
            Assert.AreEqual(HttpStatusCode.OK, navigationResponse.Status);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to dataURL and fire dataURL requests")]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            var dataUrl = "data:text/html,<div>yo</div>";
            var response = await Page.GoToAsync(dataUrl);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.AreEqual(dataUrl, requests[0].Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to URL with hash and fire requests without hash")]
        public async Task ShouldNavigateToURLWithHashAndFireRequestsWithoutHash()
        {
            var requests = new List<IRequest>();
            Page.Request += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreEqual(TestConstants.EmptyPage, response.Url);
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.AreEqual(TestConstants.EmptyPage, requests[0].Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with self requesting page")]
        public async Task ShouldWorkWithSelfRequestingPage()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/self-request.html");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            StringAssert.Contains("self-request.html", response.Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating and show the url at the error message")]
        public void ShouldFailWhenNavigatingAndShowTheUrlAtTheErrorMessage()
        {
            var url = TestConstants.HttpsPrefix + "/redirect/1.html";
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(url));
            StringAssert.Contains(url, exception.Message);
            StringAssert.Contains(url, exception.Url);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should send referer")]
        public async Task ShouldSendReferer()
        {
            string referer1 = null;
            string referer2 = null;

            await Task.WhenAll(
                Server.WaitForRequest("/grid.html", r => referer1 = r.Headers["Referer"]),
                Server.WaitForRequest("/digits/1.png", r => referer2 = r.Headers["Referer"]),
                Page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions
                {
                    Referer = "http://google.com/"
                })
            );

            Assert.AreEqual("http://google.com/", referer1);
            // Make sure subresources do not inherit referer.
            Assert.AreEqual(TestConstants.ServerUrl + "/grid.html", referer2);
        }

        [Test, Retry(2), PuppeteerTest("navigation.spec", "navigation Page.goto", "should send referer policy")]
        public async Task ShouldSendRefererPolicy()
        {
            string referer1 = null;
            string referer2 = null;

            await Task.WhenAll(
                Server.WaitForRequest("/grid.html", r => referer1 = r.Headers["Referer"]),
                Server.WaitForRequest("/digits/1.png", r => referer2 = r.Headers["Referer"]),
                Page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions
                {
                    ReferrerPolicy = "no-referer"
                })
            );

            Assert.IsNull(referer1);
            // Make sure subresources do not inherit referer.
            Assert.AreEqual(TestConstants.ServerUrl + "/grid.html", referer2);
        }
    }
}
