using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseBufferTests : PuppeteerPageBaseTest
    {
        public ResponseBufferTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Response.buffer", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.That(await response.BufferAsync(), Is.EqualTo(imageBuffer));
        }

        [Test, PuppeteerTest("network.spec", "network Response.buffer", "should work with compression")]
        public async Task ShouldWorkWithCompression()
        {
            Server.EnableGzip("/pptr.png");
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/pptr.png");
            var imageBuffer = File.ReadAllBytes("./Assets/pptr.png");
            Assert.That(await response.BufferAsync(), Is.EqualTo(imageBuffer));
        }

        [Test, PuppeteerTest("network.spec", "network Response.buffer", "should throw if the response does not have a body")]
        public async Task ShouldThrowIfTheResponseDoesNotHaveABody()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/empty.html");

            Server.SetRoute("/test.html", context =>
            {
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                context.Response.Headers["Access-Control-Allow-Headers"] = "x-ping";
                return context.Response.WriteAsync("Hello World");
            });

            var url = TestConstants.CrossProcessHttpPrefix + "/test.html";
            var responseTask = Page.WaitForResponseAsync(response =>
                response.Request.Method == HttpMethod.Options && response.Url == url);

            // Trigger a request with a preflight.
            await Page.EvaluateFunctionAsync(@"async (src) => {
                const response = await fetch(src, {
                    method: 'POST',
                    headers: { 'x-ping': 'pong' },
                });
                return response;
            }", url);

            var response = await responseTask;
            Assert.ThrowsAsync<BufferException>(async () => await response.BufferAsync());
        }
    }
}
