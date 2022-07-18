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
    public class RequestContinueTests : DevToolsContextBaseTest
    {
        public RequestContinueTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend HTTP headers")]
        [PuppeteerFact]
        public async Task ShouldAmendHTTPHeaders()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                var headers = new Dictionary<string, string>(e.Request.Headers)
                {
                    ["FOO"] = "bar"
                };
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var requestTask = Server.WaitForRequest("/sleep.zzz", request => request.Headers["foo"].ToString());
            await Task.WhenAll(
                requestTask,
                DevToolsContext.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );
            Assert.Equal("bar", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should redirect in a way non-observable to page")]
        [PuppeteerFact]
        public async Task ShouldRedirectInAWayNonObservableToPage()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                var redirectURL = e.Request.Url.Contains("/empty.html")
                    ? TestConstants.ServerUrl + "/consolelog.html" :
                    null;
                await e.Request.ContinueAsync(new Payload { Url = redirectURL });
            };
            string consoleMessage = null;
            DevToolsContext.Console += (_, e) => consoleMessage = e.Message.Text;
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, DevToolsContext.Url);
            Assert.Equal("yellow", consoleMessage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend method")]
        [PuppeteerFact]
        public async Task ShouldAmendMethodData()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.ContinueAsync(new Payload { Method = HttpMethod.Post });
            };

            var requestTask = Server.WaitForRequest<string>("/sleep.zzz", request => request.Method);

            await Task.WhenAll(
                requestTask,
                DevToolsContext.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );

            Assert.Equal("POST", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend post data")]
        [PuppeteerFact]
        public async Task ShouldAmendPostData()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.ContinueAsync(new Payload
                {
                    Method = HttpMethod.Post,
                    PostData = "doggo"
                });
            };
            var requestTask = Server.WaitForRequest("/sleep.zzz", async request =>
            {
                using (var reader = new StreamReader(request.Body, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            });

            await Task.WhenAll(
                requestTask,
                DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/sleep.zzz")
            );

            Assert.Equal("doggo", await requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend both post data and method on navigation")]
        [PuppeteerFact]
        public async Task ShouldAmendBothPostDataAndMethodOnNavigation()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync(new Payload
            {
                Method = HttpMethod.Post,
                PostData = "doggo"
            });
            var serverRequest = Server.WaitForRequest("/empty.html", req => new { req.Method, Body = new StreamReader(req.Body).ReadToEnd() });
            await Task.WhenAll(
                serverRequest,
                DevToolsContext.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal(HttpMethod.Post.Method, serverRequest.Result.Method);
            Assert.Equal("doggo", serverRequest.Result.Body);
        }
    }
}
