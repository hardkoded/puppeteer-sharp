// * MIT License
//  *
//  * Copyright (c) Darío Kondratiuk
//  *
//  * Permission is hereby granted, free of charge, to any person obtaining a copy
//  * of this software and associated documentation files (the "Software"), to deal
//  * in the Software without restriction, including without limitation the rights
//  * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  * copies of the Software, and to permit persons to whom the Software is
//  * furnished to do so, subject to the following conditions:
//  *
//  * The above copyright notice and this permission notice shall be included in all
//  * copies or substantial portions of the Software.
//  *
//  * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//  * SOFTWARE.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionExperimentalTests;

public class PageSetRequestInterceptionTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should cooperatively ${expectedAction} by priority")]
    [TestCase("abort")]
    [TestCase("continue")]
    [TestCase("respond")]
    public async Task ShouldCooperativelyActByPriority(string expectedAction)
    {
        var actionResults = new List<string>();

        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.EndsWith(".css"))
            {
                var headers = request.Headers;
                headers["xaction"] = "continue";
                return request.ContinueAsync(new Payload() { Headers = headers, },
                    expectedAction == "continue" ? 1 : 0);
            }

            return request.ContinueAsync(new Payload(), 0);
        });

        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.EndsWith(".css"))
            {
                Dictionary<string, object> headers = [];
                foreach (var kvp in request.Headers)
                {
                    headers.Add(kvp.Key, kvp.Value);
                }

                headers["xaction"] = "respond";
                return request.RespondAsync(new ResponseData() { Headers = headers, },
                    expectedAction == "respond" ? 1 : 0);
            }

            return request.ContinueAsync(new Payload(), 0);
        });

        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.EndsWith(".css"))
            {
                var headers = request.Headers;
                headers["xaction"] = "abort";
                return request.AbortAsync(RequestAbortErrorCode.Aborted, expectedAction == "abort" ? 1 : 0);
            }

            return request.ContinueAsync(new Payload(), 0);
        });

        Page.Response += (_, e) =>
        {
            e.Response.Headers.TryGetValue("xaction", out var xaction);

            if (e.Response.Url.EndsWith(".css") && !string.IsNullOrEmpty(xaction))
            {
                actionResults.Add(xaction);
            }
        };

        Page.RequestFailed += (_, e) =>
        {
            if (e.Request.Url.EndsWith(".css"))
            {
                actionResults.Add("abort");
            }
        };

        IResponse response;

        if (expectedAction == "continue")
        {
            var serverRequestTask = Server.WaitForRequest("/one-style.css", request => request.Headers["xaction"]);
            response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            await serverRequestTask;
            actionResults.Add(serverRequestTask.Result);
        }
        else
        {
            response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
        }

        Assert.That(actionResults, Has.Count.EqualTo(1));
        Assert.That(actionResults[0], Is.EqualTo(expectedAction));
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception", "should intercept")]
    public async Task ShouldIntercept()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(async request =>
        {
            if (TestUtils.IsFavicon(request))
            {
                await request.ContinueAsync(new Payload(), 0);
                return;
            }

            Assert.That(request.Url, Does.Contain("empty.html"));
            Assert.That(request.Headers, Is.Not.Null);
            Assert.That(request.Headers["user-agent"], Is.Not.Null);
            Assert.That(request.Headers["accept"], Is.Not.Null);
            Assert.That(request.Method, Is.EqualTo(HttpMethod.Get));
            Assert.That(request.PostData, Is.Null);
            Assert.That(request.IsNavigationRequest, Is.True);
            Assert.That(request.ResourceType, Is.EqualTo(ResourceType.Document));
            Assert.That(request.Frame, Is.EqualTo(Page.MainFrame));
            Assert.That(request.Frame.Url, Is.EqualTo(TestConstants.AboutBlank));
            await request.ContinueAsync(new Payload(), 0);
        });
        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(response.Ok, Is.True);

        Assert.That(response.RemoteAddress.Port, Is.EqualTo(TestConstants.Port));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work when POST is redirected with 302")]
    public async Task ShouldWorkWhenPostIsRedirectedWith302()
    {
        Server.SetRedirect("/rredirect", "/empty.html");
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(async request => await request.ContinueAsync(new Payload(), 0));

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

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work when header manipulation headers with redirect")]
    public async Task ShouldWorkWhenHeaderManipulationHeadersWithRedirect()
    {
        Server.SetRedirect("/rredirect", "/empty.html");
        await Page.SetRequestInterceptionAsync(true);

        Page.AddRequestInterceptor(async request =>
        {
            var headers = request.Headers.Clone();
            headers["foo"] = "bar";
            await request.ContinueAsync(new Payload { Headers = headers }, 0);
        });

        await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should be able to remove headers")]
    public async Task ShouldBeAbleToRemoveHeaders()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(async request =>
        {
            var headers = request.Headers.Clone();
            headers["foo"] = "bar";
            headers.Remove("origin");
            await request.ContinueAsync(new Payload { Headers = headers }, 0);
        });

        var requestTask = Server.WaitForRequest("/empty.html", request => request.Headers["origin"]);
        await Task.WhenAll(
            requestTask,
            Page.GoToAsync(TestConstants.ServerUrl + "/empty.html")
        );
        Assert.That(string.IsNullOrEmpty(requestTask.Result), Is.True);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should contain referer header")]
    public async Task ShouldContainRefererHeader()
    {
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        var requestsReadyTcs = new TaskCompletionSource<bool>();

        Page.AddRequestInterceptor(async request =>
        {
            if (!TestUtils.IsFavicon(request))
            {
                requests.Add(request);

                if (requests.Count > 1)
                {
                    requestsReadyTcs.TrySetResult(true);
                }
            }

            await request.ContinueAsync(new Payload(), 0);
        });

        await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
        await requestsReadyTcs.Task.WithTimeout();
        Assert.That(requests[1].Url, Does.Contain("/one-style.css"));
        Assert.That(requests[1].Headers["Referer"], Does.Contain("/one-style.html"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should properly return navigation response when URL has cookies")]
    public async Task ShouldProperlyReturnNavigationResponseWhenURLHasCookies()
    {
        // Setup cookie.
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.SetCookieAsync(new CookieParam { Name = "foo", Value = "bar" });

        // Setup request interception.
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));
        var response = await Page.ReloadAsync();
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should stop intercepting")]
    public async Task ShouldStopIntercepting()
    {
        await Page.SetRequestInterceptionAsync(true);

        async Task EventHandler(IRequest request)
        {
            await request.ContinueAsync(new Payload(), 0);
            Page.RemoveRequestInterceptor(EventHandler);
        }

        Page.AddRequestInterceptor(EventHandler);
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.SetRequestInterceptionAsync(false);
        await Page.GoToAsync(TestConstants.EmptyPage);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should show custom HTTP headers")]
    public async Task ShouldShowCustomHTTPHeaders()
    {
        await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string> { ["foo"] = "bar" });
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            Assert.That(request.Headers["foo"], Is.EqualTo("bar"));
            return request.ContinueAsync(new Payload(), 0);
        });
        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(response.Ok, Is.True);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with redirect inside sync XHR")]
    public async Task ShouldWorkWithRedirectInsideSyncXHR()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);
        Server.SetRedirect("/logo.png", "/pptr.png");
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));

        var status = await Page.EvaluateFunctionAsync<int>(@"async () =>
            {
                const request = new XMLHttpRequest();
                request.open('GET', '/logo.png', false);  // `false` makes the request synchronous
                request.send(null);
                return request.status;
            }");
        Assert.That(status, Is.EqualTo(200));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with custom referer headers")]
    public async Task ShouldWorkWithCustomRefererHeaders()
    {
        await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string> { ["referer"] = TestConstants.EmptyPage });
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            Assert.That(request.Headers["referer"], Is.EqualTo(TestConstants.EmptyPage));
            return request.ContinueAsync(new Payload(), 0);
        });
        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(response.Ok, Is.True);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception", "should be abortable")]
    public async Task ShouldBeAbortable()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.EndsWith(".css"))
            {
                return request.AbortAsync(RequestAbortErrorCode.Failed, 0);
            }
            else
            {
                return request.ContinueAsync(new Payload(), 0);
            }
        });
        var failedRequests = 0;
        Page.RequestFailed += (_, _) => failedRequests++;
        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
        Assert.That(response.Ok, Is.True);
        Assert.That(response.Request.FailureText, Is.Null);
        Assert.That(failedRequests, Is.EqualTo(1));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should be abortable with custom error codes")]
    public async Task ShouldBeAbortableWithCustomErrorCodes()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.AbortAsync(RequestAbortErrorCode.InternetDisconnected, 0));
        IRequest failedRequest = null;
        Page.RequestFailed += (_, e) => failedRequest = e.Request;

        var exception = Assert.ThrowsAsync<NavigationException>(
            () => Page.GoToAsync(TestConstants.EmptyPage));

        Assert.That(exception.Message, Does.StartWith("net::ERR_INTERNET_DISCONNECTED"));
        Assert.That(failedRequest, Is.Not.Null);
        Assert.That(failedRequest.FailureText, Is.EqualTo("net::ERR_INTERNET_DISCONNECTED"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception", "should send referer")]
    public async Task ShouldSendReferer()
    {
        await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string> { ["referer"] = "http://google.com/" });
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));
        var requestTask = Server.WaitForRequest("/grid.html", request => request.Headers["referer"].ToString());
        await Task.WhenAll(
            requestTask,
            Page.GoToAsync(TestConstants.ServerUrl + "/grid.html")
        );
        Assert.That(requestTask.Result, Is.EqualTo("http://google.com/"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should fail navigation when aborting main resource")]
    public async Task ShouldFailNavigationWhenAbortingMainResource()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.AbortAsync(RequestAbortErrorCode.Failed, 0));
        var exception = Assert.ThrowsAsync<NavigationException>(
            () => Page.GoToAsync(TestConstants.EmptyPage));

        if (TestConstants.IsChrome)
        {
            Assert.That(exception.Message, Does.Contain("net::ERR_FAILED"));
        }
        else
        {
            Assert.That(exception.Message, Does.Contain("NS_ERROR_FAILURE"));
        }
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with redirects")]
    public async Task ShouldWorkWithRedirects()
    {
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(async request =>
        {
            await request.ContinueAsync(new Payload(), 0);
            requests.Add(request);
        });

        Server.SetRedirect("/non-existing-page.html", "/non-existing-page-2.html");
        Server.SetRedirect("/non-existing-page-2.html", "/non-existing-page-3.html");
        Server.SetRedirect("/non-existing-page-3.html", "/non-existing-page-4.html");
        Server.SetRedirect("/non-existing-page-4.html", "/empty.html");
        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/non-existing-page.html");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Url, Does.Contain("empty.html"));
        Assert.That(requests, Has.Count.EqualTo(5));
        Assert.That(requests[2].ResourceType, Is.EqualTo(ResourceType.Document));

        // Check redirect chain
        var redirectChain = response.Request.RedirectChain;
        Assert.That(redirectChain, Has.Length.EqualTo(4));
        Assert.That(redirectChain[0].Url, Does.Contain("/non-existing-page.html"));
        Assert.That(redirectChain[2].Url, Does.Contain("/non-existing-page-3.html"));

        for (var i = 0; i < redirectChain.Length; ++i)
        {
            var request = redirectChain[i];
            Assert.That(request.IsNavigationRequest, Is.True);
            Assert.That(request.RedirectChain.ElementAt(i), Is.EqualTo(request));
        }
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with redirects for subresources")]
    public async Task ShouldWorkWithRedirectsForSubresources()
    {
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            if (!TestUtils.IsFavicon(request))
            {
                requests.Add(request);
            }

            return request.ContinueAsync(new Payload(), 0);
        });

        Server.SetRedirect("/one-style.css", "/two-style.css");
        Server.SetRedirect("/two-style.css", "/three-style.css");
        Server.SetRedirect("/three-style.css", "/four-style.css");
        Server.SetRoute("/four-style.css",
            async context => { await context.Response.WriteAsync("body {box-sizing: border-box; }"); });

        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Url, Does.Contain("one-style.html"));
        Assert.That(requests, Has.Count.EqualTo(5));
        Assert.That(requests[0].ResourceType, Is.EqualTo(ResourceType.Document));
        Assert.That(requests[1].ResourceType, Is.EqualTo(ResourceType.StyleSheet));

        // Check redirect chain
        var redirectChain = requests[1].RedirectChain;
        Assert.That(redirectChain, Has.Length.EqualTo(3));
        Assert.That(redirectChain[0].Url, Does.Contain("one-style.css"));
        Assert.That(redirectChain[2].Url, Does.Contain("three-style.css"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should be able to abort redirects")]
    public async Task ShouldBeAbleToAbortRedirects()
    {
        await Page.SetRequestInterceptionAsync(true);
        Server.SetRedirect("/non-existing.json", "/non-existing-2.json");
        Server.SetRedirect("/non-existing-2.json", "/simple.html");
        Page.AddRequestInterceptor(request =>
        {
            if (request.Url.Contains("non-existing-2"))
            {
                return request.AbortAsync(RequestAbortErrorCode.Failed, 0);
            }

            return request.ContinueAsync(new Payload(), 0);
        });

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
            Assert.That(result, Does.Contain("Failed to fetch"));
        }
        else
        {
            Assert.That(result, Does.Contain("NetworkError"));
        }
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with equal requests")]
    public async Task ShouldWorkWithEqualRequests()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);
        var responseCount = 1;
        Server.SetRoute("/zzz", context => context.Response.WriteAsync(((responseCount++) * 11) + string.Empty));
        await Page.SetRequestInterceptionAsync(true);

        var spinner = false;
        // Cancel 2nd request.
        Page.AddRequestInterceptor(request =>
        {
            if (TestUtils.IsFavicon(request))
            {
                return request.ContinueAsync(new Payload(), 0);
            }

            if (spinner)
            {
                spinner = !spinner;
                return request.AbortAsync(RequestAbortErrorCode.Failed, 0);
            }

            spinner = !spinner;
            return request.ContinueAsync(new Payload(), 0);
        });

        var results = await Page.EvaluateExpressionAsync<string[]>(@"Promise.all([
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
              fetch('/zzz').then(response => response.text()).catch(e => 'FAILED'),
            ])");
        Assert.That(results, Is.EqualTo(new[] { "11", "FAILED", "22" }));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should navigate to dataURL and fire dataURL requests")]
    public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
    {
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            requests.Add(request);
            return request.ContinueAsync(new Payload(), 0);
        });

        var dataURL = "data:text/html,<div>yo</div>";
        var response = await Page.GoToAsync(dataURL);
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requests, Has.Exactly(1).Items);
        Assert.That(requests[0].Url, Is.EqualTo(dataURL));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should be able to fetch dataURL and fire dataURL requests")]
    public async Task ShouldBeAbleToFetchDataURLAndFireDataURLRequests()
    {
        await Page.GoToAsync(TestConstants.EmptyPage);
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            if (!TestUtils.IsFavicon(request))
            {
                requests.Add(request);
            }
            return request.ContinueAsync(new Payload(), 0);
        });
        var dataURL = "data:text/html,<div>yo</div>";
        var text = await Page.EvaluateFunctionAsync<string>("url => fetch(url).then(r => r.text())", dataURL);

        Assert.That(text, Is.EqualTo("<div>yo</div>"));
        Assert.That(requests, Has.Exactly(1).Items);
        Assert.That(requests[0].Url, Is.EqualTo(dataURL));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should navigate to URL with hash and fire requests without hash")]
    public async Task ShouldNavigateToURLWithHashAndAndFireRequestsWithoutHash()
    {
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            requests.Add(request);
            return request.ContinueAsync(new Payload(), 0);
        });
        var response = await Page.GoToAsync(TestConstants.EmptyPage + "#hash");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response.Url, Is.EqualTo(TestConstants.EmptyPage));
        Assert.That(requests, Has.Exactly(1).Items);
        Assert.That(requests[0].Url, Is.EqualTo(TestConstants.EmptyPage));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with encoded server")]
    public async Task ShouldWorkWithEncodedServer()
    {
        // The requestWillBeSent will report encoded URL, whereas interception will
        // report URL as-is. @see crbug.com/759388
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));
        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/some nonexisting page");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with badly encoded server")]
    public async Task ShouldWorkWithBadlyEncodedServer()
    {
        await Page.SetRequestInterceptionAsync(true);
        Server.SetRoute("/malformed?rnd=%911", _ => Task.CompletedTask);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));
        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/malformed?rnd=%911");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with encoded server - 2")]
    public async Task ShouldWorkWithEncodedServerNegative2()
    {
        // The requestWillBeSent will report URL as-is, whereas interception will
        // report encoded URL for stylesheet. @see crbug.com/759388
        await Page.SetRequestInterceptionAsync(true);
        var requests = new List<IRequest>();
        Page.AddRequestInterceptor(request =>
        {
            requests.Add(request);
            return request.ContinueAsync(new Payload(), 0);
        });
        var response =
            await Page.GoToAsync(
                $"data:text/html,<link rel=\"stylesheet\" href=\"{TestConstants.ServerUrl}/fonts?helvetica|arial\"/>");
        Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(requests, Has.Count.EqualTo(2));
        Assert.That(requests[1].Response.Status, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should not throw \"Invalid Interception Id\" if the request was cancelled")]
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

        var _ = Page.QuerySelectorAsync("iframe")
            .EvaluateFunctionAsync<object>("(frame, url) => frame.src = url", TestConstants.ServerUrl);
        // Wait for request interception.
        await requestIntercepted.Task;
        // Delete frame to cause request to be canceled.
        _ = Page.QuerySelectorAsync("iframe").EvaluateFunctionAsync<object>("frame => frame.remove()");
        await request.ContinueAsync(new Payload(), 0);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should throw if interception is not enabled")]
    public async Task ShouldThrowIfInterceptionIsNotEnabled()
    {
        Exception exception = null;
        Page.AddRequestInterceptor(async request =>
        {
            try
            {
                await request.ContinueAsync(new Payload(), 0);
            }
            catch (Exception ex)
            {
                exception = ex;
            }
        });
        await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.That(exception.Message, Does.Contain("Request Interception is not enabled"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should work with file URLs")]
    public async Task ShouldWorkWithFileURLs()
    {
        await Page.SetRequestInterceptionAsync(true);
        var urls = new List<string>();
        Page.AddRequestInterceptor(request =>
        {
            urls.Add(request.Url.Split('/').Last());
            return request.ContinueAsync(new Payload(), 0);
        });

        var uri = new Uri(Path.Combine(Directory.GetCurrentDirectory(), "Assets", "one-style.html")).AbsoluteUri;
        await Page.GoToAsync(uri);
        Assert.That(urls, Has.Count.EqualTo(2));
        Assert.That(urls, Does.Contain("one-style.html"));
        Assert.That(urls, Does.Contain("one-style.css"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should not cache if cache disabled")]
    public async Task ShouldNotCacheIfCacheDisabled()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
        await Page.SetRequestInterceptionAsync(true);
        await Page.SetCacheEnabledAsync(false);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));

        var cached = new List<IRequest>();
        Page.RequestServedFromCache += (_, e) => cached.Add(e.Request);

        await Page.ReloadAsync();
        Assert.That(cached, Is.Empty);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should cache if cache enabled")]
    public async Task ShouldNotCacheIfCacheEnabled()
    {
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
        await Page.SetRequestInterceptionAsync(true);
        await Page.SetCacheEnabledAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));

        var cached = new List<IRequest>();
        Page.RequestServedFromCache += (_, e) => cached.Add(e.Request);

        await Page.ReloadAsync();
        Assert.That(cached, Has.Exactly(1).Items);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Page.setRequestInterception",
        "should load fonts if cache enabled")]
    public async Task ShouldLoadFontsIfCacheEnabled()
    {
        await Page.SetRequestInterceptionAsync(true);
        await Page.SetCacheEnabledAsync(true);
        Page.AddRequestInterceptor(request => request.ContinueAsync(new Payload(), 0));

        var waitTask = Page.WaitForResponseAsync(response => response.Url.EndsWith("/one-style.woff"));
        await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style-font.html");
        await waitTask;
    }
}
