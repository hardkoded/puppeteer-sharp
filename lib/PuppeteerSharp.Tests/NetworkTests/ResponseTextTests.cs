using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseTextTests : PuppeteerPageBaseTest
    {
        public ResponseTextTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.text", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.AreEqual("{\"foo\": \"bar\"}", (await response.TextAsync()).Trim());
        }

        [PuppeteerTest("network.spec.ts", "Response.text", "should return uncompressed text")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnUncompressedText()
        {
            Server.EnableGzip("/simple.json");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.AreEqual("gzip", response.Headers["Content-Encoding"]);
            Assert.AreEqual("{\"foo\": \"bar\"}", (await response.TextAsync()).Trim());
        }

        [PuppeteerTest("network.spec.ts", "Response.text", "should throw when requesting body of redirected response")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldThrowWhenRequestingBodyOfRedirectedResponse()
        {
            Server.SetRedirect("/foo.html", "/empty.html");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/foo.html");
            var redirectChain = response.Request.RedirectChain;
            Assert.Single(redirectChain);
            var redirected = redirectChain[0].Response;
            Assert.AreEqual(HttpStatusCode.Redirect, redirected.Status);

            var exception = await Assert.ThrowsAsync<PuppeteerException>(async () => await redirected.TextAsync());
            Assert.Contains("Response body is unavailable for redirect responses", exception.Message);
        }

        [PuppeteerTest("network.spec.ts", "Response.text", "should wait until response completes")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Page.RequestFinished += (_, _) => requestFinished = true;
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
