using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    public class RequestContinueTests : PuppeteerPageBaseTest
    {
        public RequestContinueTests(): base()
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend HTTP headers")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAmendHTTPHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                var headers = new Dictionary<string, string>(e.Request.Headers)
                {
                    ["FOO"] = "bar"
                };
                await e.Request.ContinueAsync(new Payload { Headers = headers });
            };
            await Page.GoToAsync(TestConstants.EmptyPage);
            var requestTask = Server.WaitForRequest("/sleep.zzz", request => request.Headers["foo"].ToString());
            await Task.WhenAll(
                requestTask,
                Page.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );
            Assert.AreEqual("bar", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should redirect in a way non-observable to page")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldRedirectInAWayNonObservableToPage()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                var redirectURL = e.Request.Url.Contains("/empty.html")
                    ? TestConstants.ServerUrl + "/consolelog.html" :
                    null;
                await e.Request.ContinueAsync(new Payload { Url = redirectURL });
            };
            string consoleMessage = null;
            Page.Console += (_, e) => consoleMessage = e.Message.Text;
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(TestConstants.EmptyPage, Page.Url);
            Assert.AreEqual("yellow", consoleMessage);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend method")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAmendMethodData()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                await e.Request.ContinueAsync(new Payload { Method = HttpMethod.Post });
            };

            var requestTask = Server.WaitForRequest<string>("/sleep.zzz", request => request.Method);

            await Task.WhenAll(
                requestTask,
                Page.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );

            Assert.AreEqual("POST", requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend post data")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAmendPostData()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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
                Page.GoToAsync(TestConstants.ServerUrl + "/sleep.zzz")
            );

            Assert.AreEqual("doggo", await requestTask.Result);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.continue", "should amend both post data and method on navigation")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAmendBothPostDataAndMethodOnNavigation()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync(new Payload
            {
                Method = HttpMethod.Post,
                PostData = "doggo"
            });
            
            var serverRequestTask = Server.WaitForRequest("/empty.html", async req =>
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                return new { req.Method, Body = body };
            });
            
            await Task.WhenAll(
                serverRequestTask,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            var serverRequest = await serverRequestTask;
            Assert.AreEqual(HttpMethod.Post.Method, serverRequest.Result.Method);
            Assert.AreEqual("doggo", serverRequest.Result.Body);
        }
    }
}
