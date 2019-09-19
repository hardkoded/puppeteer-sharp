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
using PuppeteerSharp.Helpers;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestContinueTests : PuppeteerPageBaseTest
    {
        public RequestContinueTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Fact]
        public async Task ShouldAmendHTTPHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
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
            Assert.Equal("bar", requestTask.Result);
        }

        [Fact]
        public async Task ShouldRedirectInAWayNonObservableToPage()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                var redirectURL = e.Request.Url.Contains("/empty.html")
                    ? TestConstants.ServerUrl + "/consolelog.html" :
                    null;
                await e.Request.ContinueAsync(new Payload { Url = redirectURL });
            };
            string consoleMessage = null;
            Page.Console += (sender, e) => consoleMessage = e.Message.Text;
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, Page.Url);
            Assert.Equal("yellow", consoleMessage);
        }

        [Fact]
        public async Task ShouldAmendMethodData()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                await e.Request.ContinueAsync(new Payload { Method = HttpMethod.Post });
            };

            var requestTask = Server.WaitForRequest<string>("/sleep.zzz", request => request.Method);

            await Task.WhenAll(
                requestTask,
                Page.EvaluateExpressionAsync("fetch('/sleep.zzz')")
            );

            Assert.Equal("POST", requestTask.Result);
        }

        [Fact]
        public async Task ShouldAmendPostData()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
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

            Assert.Equal("doggo", await requestTask.Result);
        }

        [Fact]
        public async Task ShouldAmendBothPostDataAndMethodOnNavigation()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) => await e.Request.ContinueAsync(new Payload
            {
                Method = HttpMethod.Post,
                PostData = "doggo"
            });
            var serverRequest = Server.WaitForRequest("/empty.html", req => new { req.Method, Body = new StreamReader(req.Body).ReadToEnd() });
            await Task.WhenAll(
                serverRequest,
                Page.GoToAsync(TestConstants.EmptyPage)
            );
            Assert.Equal(HttpMethod.Post.Method, serverRequest.Result.Method);
            Assert.Equal("doggo", serverRequest.Result.Body);
        }
    }
}