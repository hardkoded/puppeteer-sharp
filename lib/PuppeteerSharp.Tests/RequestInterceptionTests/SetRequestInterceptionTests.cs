using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.DevTools.Dom.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using CefSharp.DevTools.Dom;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetRequestInterceptionTests : DevToolsContextBaseTest
    {
        public SetRequestInterceptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should intercept")]
        [PuppeteerFact (Skip = "TODO: CEF")]
        public async Task ShouldIntercept()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                if (TestUtils.IsFavicon(e.Request))
                {
                    await e.Request.ContinueAsync();
                    return;
                }
                Assert.Contains("empty.html", e.Request.Url);
                Assert.NotNull(e.Request.Headers);
                Assert.Equal(HttpMethod.Get, e.Request.Method);
                Assert.Null(e.Request.PostData);
                Assert.True(e.Request.IsNavigationRequest);
                Assert.Equal(ResourceType.Document, e.Request.ResourceType);
                Assert.Equal(DevToolsContext.MainFrame, e.Request.Frame);
                Assert.Equal(TestConstants.AboutBlank, e.Request.Frame.Url);
                await e.Request.ContinueAsync();
            };
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);

            Assert.Equal(TestConstants.Port, response.RemoteAddress.Port);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work when POST is redirected with 302")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenPostIsEedirectedWith302()
        {
            Server.SetRedirect("/rredirect", "/empty.html");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();

            await DevToolsContext.SetContentAsync(@"
                <form action='/rredirect' method='post'>
                    <input type='hidden' id='foo' name='foo' value='FOOBAR'>
                </form>
            ");
            await Task.WhenAll(
                DevToolsContext.QuerySelectorAsync("form").EvaluateFunctionAsync("form => form.submit()"),
                DevToolsContext.WaitForNavigationAsync()
            );
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work when header manipulation headers with redirect")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenHeaderManipulationHeadersWithRedirect()
        {
            Server.SetRedirect("/rredirect", "/empty.html");
            await DevToolsContext.SetRequestInterceptionAsync(true);

            DevToolsContext.Request += async (_, e) =>
            {
                var headers = e.Request.Headers.Clone();
                headers["foo"] = "bar";
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be able to remove headers")]
        [PuppeteerFact]
        public async Task ShouldBeAbleToRemoveHeaders()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                var headers = e.Request.Headers.Clone();
                headers["foo"] = "bar";
                headers.Remove("origin");
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };

            var requestTask = Server.WaitForRequest("/empty.html", request => request.Headers["origin"]);
            await Task.WhenAll(
                requestTask,
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/empty.html")
            );
            Assert.True(string.IsNullOrEmpty(requestTask.Result));
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should contain referer header")]
        [PuppeteerFact]
        public async Task ShouldContainRefererHeader()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            var requestsReadyTcs = new TaskCompletionSource<bool>();

            DevToolsContext.Request += async (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);

                    if (requests.Count > 1)
                    {
                        requestsReadyTcs.TrySetResult(true);
                    }
                }
                await e.Request.ContinueAsync();
            };

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            await requestsReadyTcs.Task.WithTimeout();
            Assert.Contains("/one-style.css", requests[1].Url);
            Assert.Contains("/one-style.html", requests[1].Headers["Referer"]);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should properly return navigation response when URL has cookies")]
        [PuppeteerFact]
        public async Task ShouldProperlyReturnNavigationResponseWhenURLHasCookies()
        {
            // Setup cookie.
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetCookieAsync(new CookieParam
            {
                Name = "foo",
                Value = "bar"
            });

            // Setup request interception.
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += (sender, e) => _ = e.Request.ContinueAsync();
            var response = await DevToolsContext.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should stop intercepting")]
        [PuppeteerFact]
        public async Task ShouldStopIntercepting()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            async void EventHandler(object sender, RequestEventArgs e)
            {
                await e.Request.ContinueAsync();
                DevToolsContext.Request -= EventHandler;
            }
            DevToolsContext.Request += EventHandler;
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetRequestInterceptionAsync(false);
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should show custom HTTP headers")]
        [PuppeteerFact]
        public async Task ShouldShowCustomHTTPHeaders()
        {
            await DevToolsContext.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["foo"] = "bar"
            });
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                Assert.Equal("bar", e.Request.Headers["foo"]);
                await e.Request.ContinueAsync();
            };
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with redirect inside sync XHR")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRedirectInsideSyncXHR()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Server.SetRedirect("/logo.png", "/pptr.png");
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();

            var status = await DevToolsContext.EvaluateFunctionAsync<int>(@"async () =>
            {
                const request = new XMLHttpRequest();
                request.open('GET', '/logo.png', false);  // `false` makes the request synchronous
                request.send(null);
                return request.status;
            }");
            Assert.Equal(200, status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with custom referer headers")]
        [PuppeteerFact]
        public async Task ShouldWorkWithCustomRefererHeaders()
        {
            await DevToolsContext.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = TestConstants.EmptyPage
            });
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                Assert.Equal(TestConstants.EmptyPage, e.Request.Headers["referer"]);
                await e.Request.ContinueAsync();
            };
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be abortable")]
        [PuppeteerFact]
        public async Task ShouldBeAbortable()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
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
            DevToolsContext.RequestFailed += (_, _) => failedRequests++;
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.True(response.Ok);
            Assert.Null(response.Request.Failure);
            Assert.Equal(1, failedRequests);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be abortable with custom error codes")]
        [PuppeteerFact]
        public async Task ShouldBeAbortableWithCustomErrorCodes()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.AbortAsync(RequestAbortErrorCode.InternetDisconnected);
            };
            Request failedRequest = null;
            DevToolsContext.RequestFailed += (_, e) => failedRequest = e.Request;
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage).ContinueWith(_ => { });
            Assert.NotNull(failedRequest);
            Assert.Equal("net::ERR_INTERNET_DISCONNECTED", failedRequest.Failure);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should send referer")]
        [PuppeteerFact]
        public async Task ShouldSendReferer()
        {
            await DevToolsContext.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = "http://google.com/"
            });
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            var requestTask = Server.WaitForRequest("/grid.html", request => request.Headers["referer"].ToString());
            await Task.WhenAll(
                requestTask,
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/grid.html")
            );
            Assert.Equal("http://google.com/", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should fail navigation when aborting main resource")]
        [PuppeteerFact]
        public async Task ShouldFailNavigationWhenAbortingMainResource()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.AbortAsync();
            var exception = await Assert.ThrowsAsync<NavigationException>(
                () => DevToolsContext.GoToAsync(TestConstants.EmptyPage));

            Assert.Contains("net::ERR_FAILED", exception.Message);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with redirects")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRedirects()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.ContinueAsync();
                requests.Add(e.Request);
            };

            Server.SetRedirect("/non-existing-page.html", "/non-existing-page-2.html");
            Server.SetRedirect("/non-existing-page-2.html", "/non-existing-page-3.html");
            Server.SetRedirect("/non-existing-page-3.html", "/non-existing-page-4.html");
            Server.SetRedirect("/non-existing-page-4.html", "/empty.html");
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/non-existing-page.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("empty.html", response.Url);
            Assert.Equal(5, requests.Count);
            Assert.Equal(ResourceType.Document, requests[2].ResourceType);

            // Check redirect chain
            var redirectChain = response.Request.RedirectChain;
            Assert.Equal(4, redirectChain.Length);
            Assert.Contains("/non-existing-page.html", redirectChain[0].Url);
            Assert.Contains("/non-existing-page-3.html", redirectChain[2].Url);

            for (var i = 0; i < redirectChain.Length; ++i)
            {
                var request = redirectChain[i];
                Assert.True(request.IsNavigationRequest);
                Assert.Equal(request, request.RedirectChain.ElementAt(i));
            }
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with redirects for subresources")]
        [PuppeteerFact]
        public async Task ShouldWorkWithRedirectsForSubresources()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }

                await e.Request.ContinueAsync();
            };

            Server.SetRedirect("/one-style.css", "/two-style.css");
            Server.SetRedirect("/two-style.css", "/three-style.css");
            Server.SetRedirect("/three-style.css", "/four-style.css");
            Server.SetRoute("/four-style.css", async context =>
            {
                await context.Response.WriteAsync("body {box-sizing: border-box; }");
            });

            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Contains("one-style.html", response.Url);
            Assert.Equal(5, requests.Count);
            Assert.Equal(ResourceType.Document, requests[0].ResourceType);
            Assert.Equal(ResourceType.StyleSheet, requests[1].ResourceType);

            // Check redirect chain
            var redirectChain = requests[1].RedirectChain;
            Assert.Equal(3, redirectChain.Length);
            Assert.Contains("one-style.css", redirectChain[0].Url);
            Assert.Contains("three-style.css", redirectChain[2].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be able to abort redirects")]
        [PuppeteerFact]
        public async Task ShouldBeAbleToAbortRedirects()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            Server.SetRedirect("/non-existing.json", "/non-existing-2.json");
            Server.SetRedirect("/non-existing-2.json", "/simple.html");
            DevToolsContext.Request += async (_, e) =>
            {
                if (e.Request.Url.Contains("non-existing-2"))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var result = await DevToolsContext.EvaluateFunctionAsync<string>(@"async () => {
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

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with equal requests")]
        [PuppeteerFact]
        public async Task ShouldWorkWithEqualRequests()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var responseCount = 1;
            Server.SetRoute("/zzz", context => context.Response.WriteAsync(((responseCount++) * 11) + string.Empty));
            await DevToolsContext.SetRequestInterceptionAsync(true);

            var spinner = false;
            // Cancel 2nd request.
            DevToolsContext.Request += async (_, e) =>
            {
                if (TestUtils.IsFavicon(e.Request))
                {
                    await e.Request.ContinueAsync();
                    return;
                }

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
            var results = await DevToolsContext.EvaluateExpressionAsync<string[]>(@"Promise.all([
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
            ])");
            Assert.Equal(new[] { "11", "FAILED", "22" }, results);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should navigate to dataURL and fire dataURL requests")]
        [PuppeteerFact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var dataURL = "data:text/html,<div>yo</div>";
            var response = await DevToolsContext.GoToAsync(dataURL);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Single(requests);
            Assert.Equal(dataURL, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be able to fetch dataURL and fire dataURL requests")]
        [PuppeteerFact]
        public async Task ShouldBeAbleToFetchDataURLAndFireDataURLRequests()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var dataURL = "data:text/html,<div>yo</div>";
            var text = await DevToolsContext.EvaluateFunctionAsync<string>("url => fetch(url).then(r => r.text())", dataURL);

            Assert.Equal("<div>yo</div>", text);
            Assert.Single(requests);
            Assert.Equal(dataURL, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should navigate to URL with hash and fire requests without hash")]
        [PuppeteerFact]
        public async Task ShouldNavigateToURLWithHashAndAndFireRequestsWithoutHash()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with encoded server")]
        [PuppeteerFact]
        public async Task ShouldWorkWithEncodedServer()
        {
            // The requestWillBeSent will report encoded URL, whereas interception will
            // report URL as-is. @see crbug.com/759388
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/some nonexisting page");
            Assert.Equal(HttpStatusCode.NotFound, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with badly encoded server")]
        [PuppeteerFact]
        public async Task ShouldWorkWithBadlyEncodedServer()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            Server.SetRoute("/malformed?rnd=%911", _ => Task.CompletedTask);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/malformed?rnd=%911");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with encoded server - 2")]
        [PuppeteerFact]
        public async Task ShouldWorkWithEncodedServerNegative2()
        {
            // The requestWillBeSent will report URL as-is, whereas interception will
            // report encoded URL for stylesheet. @see crbug.com/759388
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var requests = new List<Request>();
            DevToolsContext.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var response = await DevToolsContext.GoToAsync($"data:text/html,<link rel=\"stylesheet\" href=\"{TestConstants.ServerUrl}/fonts?helvetica|arial\"/>");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(2, requests.Count);
            Assert.Equal(HttpStatusCode.NotFound, requests[1].Response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should not throw \"Invalid Interception Id\" if the request was cancelled")]
        [PuppeteerFact]
        public async Task ShouldNotThrowInvalidInterceptionIdIfTheRequestWasCancelled()
        {
            await DevToolsContext.SetContentAsync("<iframe></iframe>");
            await DevToolsContext.SetRequestInterceptionAsync(true);
            Request request = null;
            var requestIntercepted = new TaskCompletionSource<bool>();
            DevToolsContext.Request += (_, e) =>
            {
                request = e.Request;
                requestIntercepted.SetResult(true);
            };

            var _ = DevToolsContext.QuerySelectorAsync("iframe").EvaluateFunctionAsync<object>("(frame, url) => frame.src = url", TestConstants.ServerUrl);
            // Wait for request interception.
            await requestIntercepted.Task;
            // Delete frame to cause request to be canceled.
            _ = DevToolsContext.QuerySelectorAsync("iframe").EvaluateFunctionAsync<object>("frame => frame.remove()");
            await request.ContinueAsync();
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should throw if interception is not enabled")]
        [PuppeteerFact]
        public async Task ShouldThrowIfInterceptionIsNotEnabled()
        {
            Exception exception = null;
            DevToolsContext.Request += async (_, e) =>
            {
                try
                {
                    await e.Request.ContinueAsync();
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            };
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Contains("Request Interception is not enabled", exception.Message);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with file URLs")]
        [PuppeteerFact]
        public async Task ShouldWorkWithFileURLs()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            var urls = new List<string>();
            DevToolsContext.Request += async (_, e) =>
            {
                urls.Add(e.Request.Url.Split('/').Last());
                await e.Request.ContinueAsync();
            };

            var uri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "one-style.html")).AbsoluteUri;
            await DevToolsContext.GoToAsync(uri);
            Assert.Equal(2, urls.Count);
            Assert.Contains("one-style.html", urls);
            Assert.Contains("one-style.css", urls);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should not cache if cache disabled")]
        [PuppeteerFact]
        public async Task ShouldNotCacheIfCacheDisabled()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await DevToolsContext.SetRequestInterceptionAsync(true);
            await DevToolsContext.SetCacheEnabledAsync(false);
            var urls = new List<string>();
            DevToolsContext.Request += (_, e) => _ = e.Request.ContinueAsync();

            var cached = new List<Request>();
            DevToolsContext.RequestServedFromCache += (_, e) => cached.Add(e.Request);

            await DevToolsContext.ReloadAsync();
            Assert.Empty(cached);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should cache if cache enabled")]
        [PuppeteerFact]
        public async Task ShouldNotCacheIfCacheEnabled()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await DevToolsContext.SetRequestInterceptionAsync(true);
            await DevToolsContext.SetCacheEnabledAsync(true);
            var urls = new List<string>();
            DevToolsContext.Request += (_, e) => _ = e.Request.ContinueAsync();

            var cached = new List<Request>();
            DevToolsContext.RequestServedFromCache += (_, e) => cached.Add(e.Request);

            await DevToolsContext.ReloadAsync();
            Assert.Single(cached);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should load fonts if cache enabled")]
        [PuppeteerFact]
        public async Task ShouldLoadFontsIfCacheEnabled()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            await DevToolsContext.SetCacheEnabledAsync(true);
            var urls = new List<string>();
            DevToolsContext.Request += (_, e) => _ = e.Request.ContinueAsync();

            var waitTask = DevToolsContext.WaitForResponseAsync(response => response.Url.EndsWith("/one-style.woff"));
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style-font.html");
            await waitTask;
        }
    }
}
