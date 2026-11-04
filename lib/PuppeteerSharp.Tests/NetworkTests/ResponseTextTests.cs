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

        [Test, PuppeteerTest("network.spec", "network Response.text", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.That((await response.TextAsync()).Trim(), Is.EqualTo("{\"foo\": \"bar\"}"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.text", "should return uncompressed text")]
        public async Task ShouldReturnUncompressedText()
        {
            Server.EnableGzip("/simple.json");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            Assert.That(response.Headers["Content-Encoding"], Is.EqualTo("gzip"));
            Assert.That((await response.TextAsync()).Trim(), Is.EqualTo("{\"foo\": \"bar\"}"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.text", "should throw when requesting body of redirected response")]
        public async Task ShouldThrowWhenRequestingBodyOfRedirectedResponse()
        {
            Server.SetRedirect("/foo.html", "/empty.html");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/foo.html");
            var redirectChain = response.Request.RedirectChain;
            Assert.That(redirectChain, Has.Exactly(1).Items);
            var redirected = redirectChain[0].Response;
            Assert.That(redirected.Status, Is.EqualTo(HttpStatusCode.Redirect));

            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await redirected.TextAsync());
            Assert.That(exception.Message, Does.Contain("Response body is unavailable for redirect responses"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.text", "should wait until response completes")]
        public async Task ShouldWaitUntilResponseCompletes()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            // Setup server to trap request.
            var serverResponseTcs = new TaskCompletionSource<HttpResponse>();
            var serverResponseEnd = new TaskCompletionSource<bool>();
            Server.SetRoute("/get", async context =>
            {
                // In Firefox, |fetch| will be hanging until it receives |Content-Type| header
                // from server.
                context.Response.ContentType = "text/plain; charset=utf-8";
                await context.Response.WriteAsync("hello ");
                await context.Response.Body.FlushAsync();
                serverResponseTcs.TrySetResult(context.Response);
                await serverResponseEnd.Task;
            });
            // Setup page to trap response.
            var requestFinished = false;
            Page.RequestFinished += (_, e) => requestFinished = requestFinished || e.Request.Url.Contains("/get");
            // send request and wait for server response
            var waitForResponseTask = Page.WaitForResponseAsync(r => !TestUtils.IsFavicon(r.Request));
            await Task.WhenAll(
                waitForResponseTask,
                Page.EvaluateExpressionAsync("fetch('/get', { method: 'GET'})"),
                Server.WaitForRequest("/get"));
            var pageResponse = await waitForResponseTask;

            var serverResponse = await serverResponseTcs.Task;
            Assert.That(serverResponse, Is.Not.Null);
            Assert.That(pageResponse, Is.Not.Null);
            Assert.That(pageResponse.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(requestFinished, Is.False);

            var responseText = pageResponse.TextAsync();
            // Write part of the response and wait for it to be flushed.
            await serverResponse.WriteAsync("wor");
            await serverResponse.Body.FlushAsync();
            // Finish response.
            await serverResponse.WriteAsync("ld!");
            serverResponseEnd.TrySetResult(true);
            Assert.That(await responseText, Is.EqualTo("hello world!"));
        }
    }
}
