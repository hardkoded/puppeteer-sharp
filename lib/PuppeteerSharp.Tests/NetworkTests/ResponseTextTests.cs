using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseTextTests : PuppeteerPageBaseTest
    {
        public ResponseTextTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.text", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.AreEqual("{\"foo\": \"bar\"}", (await response.TextAsync()).Trim());
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.text", "should return uncompressed text")]
        public async Task ShouldReturnUncompressedText()
        {
            Server.EnableGzip("/simple.json");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.AreEqual("gzip", response.Headers["Content-Encoding"]);
            Assert.AreEqual("{\"foo\": \"bar\"}", (await response.TextAsync()).Trim());
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.text", "should throw when requesting body of redirected response")]
        public async Task ShouldThrowWhenRequestingBodyOfRedirectedResponse()
        {
            Server.SetRedirect("/foo.html", "/empty.html");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/foo.html");
            var redirectChain = response.Request.RedirectChain;
            Assert.That(redirectChain, Has.Exactly(1).Items);
            var redirected = redirectChain[0].Response;
            Assert.AreEqual(HttpStatusCode.Redirect, redirected.Status);

            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await redirected.TextAsync());
            StringAssert.Contains("Response body is unavailable for redirect responses", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.text", "should wait until response completes")]
        public async Task ShouldWaitUntilResponseCompletes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Setup server to trap request.
            var serverResponseCompletion = new TaskCompletionSource<bool>();
            HttpResponse serverResponse = null;
            Server.SetRoute("/get", context =>
            {
                serverResponse = context.Response;
                context.Response.WriteAsync("hello ");
                return serverResponseCompletion.Task;
            });
            // Setup page to trap response.
            IResponse pageResponse = null;
            var requestFinished = false;
            Page.Response += (_, e) => pageResponse = e.Response;
            Page.RequestFinished += (_, _) => requestFinished = requestFinished || pageResponse.Url.Contains("/get");
            // send request and wait for server response
            Task WaitForPageResponseEvent()
            {
                var completion = new TaskCompletionSource<bool>();
                Page.Response += (_, e) =>
                {
                    if (!TestUtils.IsFavicon(e.Response.Request))
                    {
                        completion.SetResult(true);
                    }
                };
                return completion.Task;
            }

            await Task.WhenAll(
                Server.WaitForRequest("/get"),
                Page.EvaluateExpressionAsync("fetch('/get', { method: 'GET'})"),
                WaitForPageResponseEvent()
            );

            Assert.NotNull(serverResponse);
            Assert.NotNull(pageResponse);
            Assert.AreEqual(HttpStatusCode.OK, pageResponse.Status);
            Assert.False(requestFinished);

            var responseText = pageResponse.TextAsync();
            // Write part of the response and wait for it to be flushed.
            await serverResponse.WriteAsync("wor");
            // Finish response.
            await serverResponse.WriteAsync("ld!");
            serverResponseCompletion.SetResult(true);
            Assert.AreEqual("hello world!", await responseText);
        }
    }
}
