using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseStreamTests : PuppeteerPageBaseTest
    {
        public ResponseStreamTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Response.stream", "should stream the body while it is loading")]
        public async Task ShouldStreamTheBodyWhileItIsLoading()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/stream", async context =>
            {
                context.Response.Headers["content-type"] = "text/plain";
                for (var i = 0; i < 4; i++)
                {
                    await context.Response.WriteAsync($"chunk{i}\n");
                    await context.Response.Body.FlushAsync();
                    if (i < 3)
                    {
                        await Task.Delay(100);
                    }
                }
            });

            var responseTask = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/stream");
            var fetchTask = Page.EvaluateFunctionAsync("async () => { await (await fetch('/stream')).text(); }");

            var response = await responseTask;
            var stream = await response.StreamAsync();
            var chunks = await ReadChunksAsync(stream);
            await fetchTask;

            Assert.That(chunks.Count, Is.GreaterThan(1));
            Assert.That(Encoding.UTF8.GetString(Concat(chunks)), Is.EqualTo("chunk0\nchunk1\nchunk2\nchunk3\n"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.stream", "should return the whole body if loading has finished")]
        public async Task ShouldReturnTheWholeBodyIfLoadingHasFinished()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/simple.json");
            var stream = await response.StreamAsync();
            var chunks = await ReadChunksAsync(stream);

            Assert.That(Encoding.UTF8.GetString(Concat(chunks)), Is.EqualTo(await response.TextAsync()));
        }

        [Test, PuppeteerTest("network.spec", "network Response.stream", "should error the stream if loading fails")]
        public async Task ShouldErrorTheStreamIfLoadingFails()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/stream", async context =>
            {
                context.Response.Headers["content-type"] = "text/plain";
                await context.Response.WriteAsync("chunk0\n");
                await context.Response.Body.FlushAsync();
                await Task.Delay(100);
                context.Abort();
            });

            var responseTask = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/stream");
            var fetchTask = Page.EvaluateFunctionAsync("async () => { try { await (await fetch('/stream')).text(); } catch (e) {} }");

            var response = await responseTask;
            var stream = await response.StreamAsync();
            var exception = Assert.ThrowsAsync<PuppeteerException>(async () => await ReadChunksAsync(stream));
            await fetchTask;

            Assert.That(exception.Message, Does.Contain("Loading failed"));
        }

        [Test, PuppeteerTest("network.spec", "network Response.stream", "should stop listening when the stream is cancelled")]
        public async Task ShouldStopListeningWhenTheStreamIsCancelled()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var release = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Server.SetRoute("/stream", async context =>
            {
                context.Response.Headers["content-type"] = "text/plain";
                await context.Response.WriteAsync("chunk0\n");
                await context.Response.Body.FlushAsync();
                await release.Task;
                await context.Response.WriteAsync("chunk1\n");
            });

            var responseTask = Page.WaitForResponseAsync(TestConstants.ServerUrl + "/stream");
            var fetchTask = Page.EvaluateFunctionAsync("async () => { await (await fetch('/stream')).text(); }");

            var response = await responseTask;
            using (var stream = await response.StreamAsync())
            {
                var buffer = new byte[1024];
                Assert.That(await stream.ReadAsync(buffer, 0, buffer.Length), Is.GreaterThan(0));
            }

            release.SetResult(true);
            await fetchTask;
        }

        private static async Task<List<byte[]>> ReadChunksAsync(Stream stream)
        {
            var chunks = new List<byte[]>();
            var buffer = new byte[1024];
            int read;
            while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                var chunk = new byte[read];
                Buffer.BlockCopy(buffer, 0, chunk, 0, read);
                chunks.Add(chunk);
            }

            return chunks;
        }

        private static byte[] Concat(List<byte[]> chunks)
        {
            using var memory = new MemoryStream();
            foreach (var chunk in chunks)
            {
                memory.Write(chunk, 0, chunk.Length);
            }

            return memory.ToArray();
        }
    }
}
