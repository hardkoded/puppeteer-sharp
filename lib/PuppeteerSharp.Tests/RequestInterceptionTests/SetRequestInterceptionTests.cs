using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetRequestInterceptionTests : PuppeteerPageBaseTest
    {
        public SetRequestInterceptionTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should intercept")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldIntercept()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                if (TestUtils.IsFavicon(e.Request))
                {
                    await e.Request.ContinueAsync();
                    return;
                }
                Assert.Contains("empty.html", e.Request.Url);
                Assert.NotNull(e.Request.Headers);
                Assert.NotNull(e.Request.Headers["user-agent"]);
                Assert.NotNull(e.Request.Headers["accept"]);
                Assert.Equal(HttpMethod.Get, e.Request.Method);
                Assert.Null(e.Request.PostData);
                Assert.True(e.Request.IsNavigationRequest);
                Assert.Equal(ResourceType.Document, e.Request.ResourceType);
                Assert.Equal(Page.MainFrame, e.Request.Frame);
                Assert.Equal(TestConstants.AboutBlank, e.Request.Frame.Url);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);

            Assert.Equal(TestConstants.Port, response.RemoteAddress.Port);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work when POST is redirected with 302")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWhenPostIsEedirectedWith302()
        {
            Server.SetRedirect("/rredirect", "/empty.html");
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();

            await Page.SetContentAsync(@"
                <form action='/rredirect' method='post'>
                    <input type='hidden' id='foo' name='foo' value='FOOBAR'>
                </form>
            ");
            await Task.WhenAll(
                Page.QuerySelectorAsync("form").EvaluateFunctionAsync("form => form.submit()"),
                Page.WaitForNavigationAsync()
            );
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work when header manipulation headers with redirect")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWhenHeaderManipulationHeadersWithRedirect()
        {
            Server.SetRedirect("/rredirect", "/empty.html");
            await Page.SetRequestInterceptionAsync(true);

            Page.Request += async (_, e) =>
            {
                var headers = e.Request.Headers.Clone();
                headers["foo"] = "bar";
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };

            await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be able to remove headers")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbleToRemoveHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                var headers = e.Request.Headers.Clone();
                headers["foo"] = "bar";
                headers.Remove("origin");
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };

            var requestTask = Server.WaitForRequest("/empty.html", request => request.Headers["origin"]);
            await Task.WhenAll(
                requestTask,
                Page.GoToAsync(TestConstants.ServerUrl + "/empty.html")
            );
            Assert.True(string.IsNullOrEmpty(requestTask.Result));
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should contain referer header")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldContainRefererHeader()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            var requestsReadyTcs = new TaskCompletionSource<bool>();

            Page.Request += async (_, e) =>
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

            await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            await requestsReadyTcs.Task.WithTimeout();
            Assert.Contains("/one-style.css", requests[1].Url);
            Assert.Contains("/one-style.html", requests[1].Headers["Referer"]);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should properly return navigation response when URL has cookies")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldProperlyReturnNavigationResponseWhenURLHasCookies()
        {
            // Setup cookie.
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetCookieAsync(new CookieParam
            {
                Name = "foo",
                Value = "bar"
            });

            // Setup request interception.
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += (sender, e) => _ = e.Request.ContinueAsync();
            var response = await Page.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should stop intercepting")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldStopIntercepting()
        {
            await Page.SetRequestInterceptionAsync(true);
            async void EventHandler(object sender, RequestEventArgs e)
            {
                await e.Request.ContinueAsync();
                Page.Request -= EventHandler;
            }
            Page.Request += EventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should show custom HTTP headers")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldShowCustomHTTPHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["foo"] = "bar"
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                Assert.Equal("bar", e.Request.Headers["foo"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with redirect inside sync XHR")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithRedirectInsideSyncXHR()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRedirect("/logo.png", "/pptr.png");
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();

            var status = await Page.EvaluateFunctionAsync<int>(@"async () =>
            {
                const request = new XMLHttpRequest();
                request.open('GET', '/logo.png', false);  // `false` makes the request synchronous
                request.send(null);
                return request.status;
            }");
            Assert.Equal(200, status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with custom referer headers")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithCustomRefererHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = TestConstants.EmptyPage
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                Assert.Equal(TestConstants.EmptyPage, e.Request.Headers["referer"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be abortable")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbortable()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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
            Page.RequestFailed += (_, _) => failedRequests++;
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.True(response.Ok);
            Assert.Null(response.Request.Failure);
            Assert.Equal(1, failedRequests);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be abortable with custom error codes")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbortableWithCustomErrorCodes()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                await e.Request.AbortAsync(RequestAbortErrorCode.InternetDisconnected);
            };
            IRequest failedRequest = null;
            Page.RequestFailed += (_, e) => failedRequest = e.Request;

            var exception = await Assert.ThrowsAsync<NavigationException>(
                () => Page.GoToAsync(TestConstants.EmptyPage));

            Assert.StartsWith("net::ERR_INTERNET_DISCONNECTED", exception.Message);
            Assert.NotNull(failedRequest);
            Assert.Equal("net::ERR_INTERNET_DISCONNECTED", failedRequest.Failure);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should send referer")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldSendReferer()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = "http://google.com/"
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var requestTask = Server.WaitForRequest("/grid.html", request => request.Headers["referer"].ToString());
            await Task.WhenAll(
                requestTask,
                Page.GoToAsync(TestConstants.ServerUrl + "/grid.html")
            );
            Assert.Equal("http://google.com/", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should fail navigation when aborting main resource")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFailNavigationWhenAbortingMainResource()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.AbortAsync();
            var exception = await Assert.ThrowsAsync<NavigationException>(
                () => Page.GoToAsync(TestConstants.EmptyPage));

            if (TestConstants.IsChrome)
            {
                Assert.Contains("net::ERR_FAILED", exception.Message);
            }
            else
            {
                Assert.Contains("NS_ERROR_FAILURE", exception.Message);
            }
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with redirects")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithRedirects()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
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
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithRedirectsForSubresources()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
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

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
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
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbleToAbortRedirects()
        {
            await Page.SetRequestInterceptionAsync(true);
            Server.SetRedirect("/non-existing.json", "/non-existing-2.json");
            Server.SetRedirect("/non-existing-2.json", "/simple.html");
            Page.Request += async (_, e) =>
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

            if (TestConstants.IsChrome)
            {
                Assert.Contains("Failed to fetch", result);
            }
            else
            {
                Assert.Contains("NetworkError", result);
            }
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with equal requests")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithEqualRequests()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var responseCount = 1;
            Server.SetRoute("/zzz", context => context.Response.WriteAsync(((responseCount++) * 11) + string.Empty));
            await Page.SetRequestInterceptionAsync(true);

            var spinner = false;
            // Cancel 2nd request.
            Page.Request += async (_, e) =>
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
            var results = await Page.EvaluateExpressionAsync<string[]>(@"Promise.all([
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
            ])");
            Assert.Equal(new[] { "11", "FAILED", "22" }, results);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should navigate to dataURL and fire dataURL requests")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var dataURL = "data:text/html,<div>yo</div>";
            var response = await Page.GoToAsync(dataURL);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Single(requests);
            Assert.Equal(dataURL, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should be able to fetch dataURL and fire dataURL requests")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeAbleToFetchDataURLAndFireDataURLRequests()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var dataURL = "data:text/html,<div>yo</div>";
            var text = await Page.EvaluateFunctionAsync<string>("url => fetch(url).then(r => r.text())", dataURL);

            Assert.Equal("<div>yo</div>", text);
            Assert.Single(requests);
            Assert.Equal(dataURL, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should navigate to URL with hash and fire requests without hash")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNavigateToURLWithHashAndAndFireRequestsWithoutHash()
        {
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with encoded server")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithEncodedServer()
        {
            // The requestWillBeSent will report encoded URL, whereas interception will
            // report URL as-is. @see crbug.com/759388
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/some nonexisting page");
            Assert.Equal(HttpStatusCode.NotFound, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with badly encoded server")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithBadlyEncodedServer()
        {
            await Page.SetRequestInterceptionAsync(true);
            Server.SetRoute("/malformed?rnd=%911", _ => Task.CompletedTask);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/malformed?rnd=%911");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with encoded server - 2")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithEncodedServerNegative2()
        {
            // The requestWillBeSent will report URL as-is, whereas interception will
            // report encoded URL for stylesheet. @see crbug.com/759388
            await Page.SetRequestInterceptionAsync(true);
            var requests = new List<IRequest>();
            Page.Request += async (_, e) =>
            {
                requests.Add(e.Request);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync($"data:text/html,<link rel=\"stylesheet\" href=\"{TestConstants.ServerUrl}/fonts?helvetica|arial\"/>");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(2, requests.Count);
            Assert.Equal(HttpStatusCode.NotFound, requests[1].Response.Status);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should not throw \"Invalid Interception Id\" if the request was cancelled")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotThrowInvalidInterceptionIdIfTheRequestWasCancelled()
        {
            await Page.SetContentAsync("<iframe></iframe>");
            await Page.SetRequestInterceptionAsync(true);
            IRequest request = null;
            var requestIntercepted = new TaskCompletionSource<bool>();
            Page.Request += (_, e) =>
            {
                request = e.Request;
                requestIntercepted.SetResult(true);
            };

            var _ = Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync<object>("(frame, url) => frame.src = url", TestConstants.ServerUrl);
            // Wait for request interception.
            await requestIntercepted.Task;
            // Delete frame to cause request to be canceled.
            _ = Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync<object>("frame => frame.remove()");
            await request.ContinueAsync();
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should throw if interception is not enabled")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldThrowIfInterceptionIsNotEnabled()
        {
            Exception exception = null;
            Page.Request += async (_, e) =>
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
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Contains("Request Interception is not enabled", exception.Message);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should work with file URLs")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithFileURLs()
        {
            await Page.SetRequestInterceptionAsync(true);
            var urls = new List<string>();
            Page.Request += async (_, e) =>
            {
                urls.Add(e.Request.Url.Split('/').Last());
                await e.Request.ContinueAsync();
            };

            var uri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "one-style.html")).AbsoluteUri;
            await Page.GoToAsync(uri);
            Assert.Equal(2, urls.Count);
            Assert.Contains("one-style.html", urls);
            Assert.Contains("one-style.css", urls);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should not cache if cache disabled")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotCacheIfCacheDisabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await Page.SetRequestInterceptionAsync(true);
            await Page.SetCacheEnabledAsync(false);
            var urls = new List<string>();
            Page.Request += (_, e) => _ = e.Request.ContinueAsync();

            var cached = new List<IRequest>();
            Page.RequestServedFromCache += (_, e) => cached.Add(e.Request);

            await Page.ReloadAsync();
            Assert.Empty(cached);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should cache if cache enabled")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldNotCacheIfCacheEnabled()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await Page.SetRequestInterceptionAsync(true);
            await Page.SetCacheEnabledAsync(true);
            var urls = new List<string>();
            Page.Request += (_, e) => _ = e.Request.ContinueAsync();

            var cached = new List<IRequest>();
            Page.RequestServedFromCache += (_, e) => cached.Add(e.Request);

            await Page.ReloadAsync();
            Assert.Single(cached);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Page.setRequestInterception", "should load fonts if cache enabled")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldLoadFontsIfCacheEnabled()
        {
            await Page.SetRequestInterceptionAsync(true);
            await Page.SetCacheEnabledAsync(true);
            var urls = new List<string>();
            Page.Request += (_, e) => _ = e.Request.ContinueAsync();

            var waitTask = Page.WaitForResponseAsync(response => response.Url.EndsWith("/one-style.woff"));
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style-font.html");
            await waitTask;
        }
    }
}
