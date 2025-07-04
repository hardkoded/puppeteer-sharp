using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    public class RequestContinueTests : PuppeteerPageBaseTest
    {
        public RequestContinueTests() : base()
        {
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should amend HTTP headers")]
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
            Assert.That(requestTask.Result, Is.EqualTo("bar"));
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should redirect in a way non-observable to page")]
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
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
            Assert.That(consoleMessage, Is.EqualTo("yellow"));
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should amend method")]
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

            Assert.That(requestTask.Result, Is.EqualTo("POST"));
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should amend post data")]
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

            Assert.That(await requestTask.Result, Is.EqualTo("doggo"));
        }

        [Test, PuppeteerTest("requestinterception.spec", "Request.continue", "should amend both post data and method on navigation")]
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
            Assert.That(serverRequest.Result.Method, Is.EqualTo(HttpMethod.Post.Method));
            Assert.That(serverRequest.Result.Body, Is.EqualTo("doggo"));
        }
    }
}
