using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseEncodedDataLengthTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Response.encodedDataLength", "should return the encoded transfer size")]
        public async Task ShouldReturnTheEncodedTransferSize()
        {
            var raw = string.Join("\n", Enumerable.Range(0, 1000).Select(i => $"line-number-{i}-some-repeated-text"));
            var rawBytes = Encoding.UTF8.GetBytes(raw);
            byte[] compressed;
            using (var memory = new MemoryStream())
            {
                using (var gzip = new GZipStream(memory, CompressionLevel.Optimal, leaveOpen: true))
                {
                    gzip.Write(rawBytes, 0, rawBytes.Length);
                }

                compressed = memory.ToArray();
            }

            Server.SetRoute("/compressible", context =>
            {
                context.Response.Headers["Content-Type"] = "text/plain";
                context.Response.Headers["Content-Encoding"] = "gzip";
                return context.Response.Body.WriteAsync(compressed, 0, compressed.Length);
            });

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/compressible");
            var responseBuffer = await response.BufferAsync();
            Assert.That(responseBuffer, Has.Length.EqualTo(rawBytes.Length));
            Assert.That(response.EncodedDataLength, Is.GreaterThanOrEqualTo(compressed.Length));
            Assert.That(response.EncodedDataLength, Is.LessThan(rawBytes.Length));
        }

        [Test, PuppeteerTest("network.spec", "network Response.encodedDataLength", "should return 0 for cached responses")]
        public async Task ShouldReturnZeroForCachedResponses()
        {
            var url = TestConstants.ServerUrl + "/cached-encoded-length";
            Server.SetRoute("/cached-encoded-length", context =>
            {
                context.Response.Headers["Cache-Control"] = "public, max-age=3600";
                context.Response.Headers["Content-Type"] = "text/plain";
                return context.Response.WriteAsync(string.Concat(Enumerable.Repeat("cached-content-", 100)));
            });

            var firstResponse = await Page.GoToAsync(url);
            await firstResponse.BufferAsync();
            Assert.That(firstResponse.FromCache, Is.False);
            Assert.That(firstResponse.EncodedDataLength, Is.GreaterThan(1500));

            var cachedRequestFinished = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            void OnRequestFinished(object sender, RequestEventArgs e)
            {
                if (e.Request.Url == url)
                {
                    cachedRequestFinished.TrySetResult(true);
                }
            }

            Page.RequestFinished += OnRequestFinished;
            try
            {
                var cachedResponse = await Page.GoToAsync(url);
                await cachedRequestFinished.Task.WaitAsync(TimeSpan.FromMilliseconds(TestConstants.DefaultPuppeteerTimeout));
                Assert.That(cachedResponse.FromCache, Is.True);
                Assert.That(cachedResponse.EncodedDataLength, Is.EqualTo(0));
            }
            finally
            {
                Page.RequestFinished -= OnRequestFinished;
            }
        }

        [Test, PuppeteerTest("network.spec", "network Response.encodedDataLength", "should not return a negative value for data: URLs")]
        public async Task ShouldNotReturnANegativeValueForDataUrls()
        {
            long? responseLengthOnEvent = null;
            Page.Response += (_, e) =>
            {
                if (e.Response.Url == "data:text/plain,hello")
                {
                    responseLengthOnEvent = e.Response.EncodedDataLength;
                }
            };

            var response = await Page.GoToAsync("data:text/plain,hello");
            Assert.That(responseLengthOnEvent, Is.Not.Null);
            Assert.That(responseLengthOnEvent, Is.GreaterThanOrEqualTo(0));
            Assert.That(response.EncodedDataLength, Is.GreaterThanOrEqualTo(0));
        }
    }
}
