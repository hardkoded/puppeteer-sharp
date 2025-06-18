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
        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with anchor navigation")]
        public async Task ShouldWorkWithAnchorNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
            await Page.GoToAsync($"{TestConstants.EmptyPage}#foo");
            Assert.That(Page.Url, Is.EqualTo($"{TestConstants.EmptyPage}#foo"));
            await Page.GoToAsync($"{TestConstants.EmptyPage}#bar");
            Assert.That(Page.Url, Is.EqualTo($"{TestConstants.EmptyPage}#bar"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with redirects")]
        public async Task ShouldWorkWithRedirects()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/empty.html");

            await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to about:blank")]
        public async Task ShouldNavigateToAboutBlank()
        {
            var response = await Page.GoToAsync(TestConstants.AboutBlank);
            Assert.That(response, Is.Null);
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should return response when page changes its URL after load")]
        public async Task ShouldReturnResponseWhenPageChangesItsURLAfterLoad()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/historyapi.html");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with subframes return 204")]
        public async Task ShouldWorkWithSubframesReturn204()
        {
            Server.SetRoute("/frames/frame.html", context =>
            {
                context.Response.StatusCode = 204;
                return Task.CompletedTask;
            });
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when server returns 204")]
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
                Assert.That(exception.Message, Does.Contain("net::ERR_ABORTED"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("NS_BINDING_ABORTED"));
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with domcontentloaded")]
        public async Task ShouldNavigateToEmptyPageWithDOMContentLoaded()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, waitUntil: new[]
            {
                WaitUntilNavigation.DOMContentLoaded
            });
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.SecurityDetails, Is.Null);
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when page calls history API in beforeunload")]
        public async Task ShouldWorkWhenPageCallsHistoryAPIInBeforeunload()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() =>
            {
                window.addEventListener('beforeunload', () => history.replaceState(null, 'initial', window.location.href), false);
            }");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when reload causes history API in beforeunload")]
        public async Task ShouldWorkWhenReloadCausesHistoryAPIInBeforeunload()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync(@"() =>
            {
                window.addEventListener('beforeunload', () => history.replaceState(null, 'initial', window.location.href), false);
            }");
            var response = await Page.ReloadAsync();
            Assert.That(await Page.EvaluateFunctionAsync<int>("() => 1"), Is.EqualTo(1));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with networkidle0")]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle0()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions
            {
                WaitUntil =
                [WaitUntilNavigation.Networkidle0]
            });
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }


        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to page with iframe and networkidle0")]
        public async Task ShouldNavigateToPageWithIframeAndNetworkidle0()

        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html", new NavigationOptions
            {
                WaitUntil =
                [WaitUntilNavigation.Networkidle0]
            });
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to empty page with networkidle2")]
        public async Task ShouldNavigateToEmptyPageWithNetworkidle2()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { WaitUntilNavigation.Networkidle2 } });
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad url")]
        public void ShouldFailWhenNavigatingToBadUrl()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync("asdfasdf"));

            if (TestConstants.IsChrome)
            {
                Assert.That(exception.Message, Does.Contain("Cannot navigate to invalid URL"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("invalid URL"));
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad SSL")]
        public void ShouldFailWhenNavigatingToBadSSL()
        {
            Page.Request += (_, e) => Assert.That(e.Request, Is.Not.Null);
            Page.RequestFinished += (_, e) => Assert.That(e.Request, Is.Not.Null);
            Page.RequestFailed += (_, e) => Assert.That(e.Request, Is.Not.Null);

            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));

            if (TestConstants.IsChrome)
            {
                Assert.That(exception.Message, Does.Contain("net::ERR_CERT_AUTHORITY_INVALID"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("SSL_ERROR_UNKNOWN"));
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating to bad SSL after redirects")]
        public void ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));

            if (TestConstants.IsChrome)
            {
                Assert.That(exception.Message, Does.Contain("net::ERR_CERT_AUTHORITY_INVALID"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("SSL_ERROR_UNKNOWN"));
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when main resources failed to load")]
        public void ShouldFailWhenMainResourcesFailedToLoad()
        {
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync("http://localhost:44123/non-existing-url"));

            if (TestConstants.IsChrome)
            {
                Assert.That(exception.Message, Does.Contain("net::ERR_CONNECTION_REFUSED"));
            }
            else
            {
                Assert.That(exception.Message, Does.Contain("NS_ERROR_CONNECTION_REFUSED"));
            }
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding maximum navigation timeout")]
        public void ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            var exception = Assert.ThrowsAsync<NavigationException>(async ()
                => await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { Timeout = 1 }));
            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding default maximum navigation timeout")]
        public void ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));

            Page.DefaultNavigationTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when exceeding default maximum timeout")]
        public void ShouldFailWhenExceedingDefaultMaximumTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should prioritize default navigation timeout over default timeout")]
        public void ShouldPrioritizeDefaultNavigationTimeoutOverDefaultTimeout()
        {
            // Hang for request to the empty.html
            Server.SetRoute("/empty.html", _ => Task.Delay(-1));
            Page.DefaultTimeout = 0;
            Page.DefaultNavigationTimeout = 1;
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(TestConstants.EmptyPage));
            Assert.That(exception.Message, Does.Contain("Timeout of 1 ms exceeded"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should disable timeout when its set to 0")]
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
            Assert.That(loaded, Is.True);
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to valid url")]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to data url")]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            var response = await Page.GoToAsync("data:text/html,hello");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work when navigating to 404")]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/not-found");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should not throw an error for a 404 response with an empty body")]
        public async Task ShouldNotThrowAnErrorForA404ResponseWithAnEmptyBody()
        {
            Server.SetRoute("/404-error", context =>
            {
                context.Response.StatusCode = 404;
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/404-error");
            Assert.That(response.Ok, Is.False);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should not throw an error for a 500 response with an empty body")]
        public async Task ShouldNotThrowAnErrorForA500ResponseWithAnEmptyBody()
        {
            Server.SetRoute("/500-error", context =>
            {
                context.Response.StatusCode = 500;
                return Task.CompletedTask;
            });

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/500-error");
            Assert.That(response.Ok, Is.False);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.InternalServerError));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should return last response in redirect chain")]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            Server.SetRedirect("/redirect/1.html", "/redirect/2.html");
            Server.SetRedirect("/redirect/2.html", "/redirect/3.html");
            Server.SetRedirect("/redirect/3.html", TestConstants.EmptyPage);

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should wait for network idle to succeed navigation")]
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

            Assert.That(navigationFinished, Is.False);

            await initialFetchResourcesRequested.WithTimeout();

            Assert.That(navigationFinished, Is.False);

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

            Assert.That(navigationFinished, Is.False);

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
            Assert.That(navigationResponse.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to dataURL and fire dataURL requests")]
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
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.That(requests[0].Url, Is.EqualTo(dataUrl));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should navigate to URL with hash and fire requests without hash")]
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
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(requests, Has.Exactly(1).Items);
            Assert.That(requests[0].Url, Is.EqualTo(TestConstants.EmptyPage));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should work with self requesting page")]
        public async Task ShouldWorkWithSelfRequestingPage()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/self-request.html");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.Url, Does.Contain("self-request.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should fail when navigating and show the url at the error message")]
        public void ShouldFailWhenNavigatingAndShowTheUrlAtTheErrorMessage()
        {
            var url = TestConstants.HttpsPrefix + "/redirect/1.html";
            var exception = Assert.ThrowsAsync<NavigationException>(async () => await Page.GoToAsync(url));
            Assert.That(exception.Message, Does.Contain(url));
            Assert.That(exception.Url, Does.Contain(url));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should send referer")]
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

            Assert.That(referer1, Is.EqualTo("http://google.com/"));
            // Make sure subresources do not inherit referer.
            Assert.That(referer2, Is.EqualTo(TestConstants.ServerUrl + "/grid.html"));
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.goto", "should send referer policy")]
        public async Task ShouldSendRefererPolicy()
        {
            string referer1 = null;
            string referer2 = null;

            await Task.WhenAll(
                Server.WaitForRequest("/grid.html", r => referer1 = r.Headers["Referer"]),
                Server.WaitForRequest("/digits/1.png", r => referer2 = r.Headers["Referer"]),
                Page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions
                {
                    ReferrerPolicy = "no-referrer"
                })
            );

            Assert.That(referer1, Is.Null);
            // Make sure subresources do not inherit referer.
            Assert.That(referer2, Is.EqualTo(TestConstants.ServerUrl + "/grid.html"));
        }
    }
}
