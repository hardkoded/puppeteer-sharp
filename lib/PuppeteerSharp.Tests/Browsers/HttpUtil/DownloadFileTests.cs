using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.Browsers.HttpUtil
{
    public class DownloadFileTests
    {
        private static readonly byte[] _testContent = TestHttpServer.TestContent;

        private string _tmpDir;

        [SetUp]
        public void CreateDir()
        {
            _tmpDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(_tmpDir);
        }

        [TearDown]
        public void DeleteDir() => Directory.Delete(_tmpDir, true);

        [
            Test,
            PuppeteerTest(
                "httpUtil.test",
                "downloadFile",
                "retries download when previous attempts fail"
            )
        ]
        public async Task RetriesDownloadWhenPreviousAttemptsFail()
        {
            using var server = new TestHttpServer(attempt => attempt == 1 ? 500 : 200);
            var destPath = Path.Combine(_tmpDir, "retry-download.bin");

            await Helpers.HttpUtil.DownloadFileAsync(server.Url, destPath, null, 1, 0);

            Assert.That(server.Requests, Is.EqualTo(2));
            Assert.That(File.ReadAllBytes(destPath), Is.EqualTo(_testContent));
        }

        [
            Test,
            PuppeteerTest("httpUtil.test", "downloadFile", "retries download on connection reset")
        ]
        public async Task RetriesDownloadOnConnectionReset()
        {
            using var server = new TestHttpServer(attempt => attempt == 1 ? 0 : 200);
            var destPath = Path.Combine(_tmpDir, "reset-download.bin");

            await Helpers.HttpUtil.DownloadFileAsync(server.Url, destPath, null, 1, 0);

            Assert.That(server.Requests, Is.EqualTo(2));
            Assert.That(File.ReadAllBytes(destPath), Is.EqualTo(_testContent));
        }

        [Test]
        public async Task Partial_RetriesDownloadWhenARequestTimesOut()
        {
            using var server = new TestHttpServer(attempt => attempt == 1 ? -1 : 200);
            var destPath = Path.Combine(_tmpDir, "timeout-download.bin");

            await Helpers.HttpUtil.DownloadFileAsync(
                server.Url,
                destPath,
                null,
                1,
                0,
                TimeSpan.FromMilliseconds(500)
            );

            Assert.That(server.Requests, Is.EqualTo(2));
            Assert.That(File.ReadAllBytes(destPath), Is.EqualTo(_testContent));
        }

        [Test]
        public void Partial_DoesNotRetryATimeoutWhenRetryIsZero()
        {
            using var server = new TestHttpServer(_ => -1);
            var destPath = Path.Combine(_tmpDir, "timeout-single.bin");

            Assert.CatchAsync<OperationCanceledException>(() =>
                Helpers.HttpUtil.DownloadFileAsync(
                    server.Url,
                    destPath,
                    null,
                    0,
                    0,
                    TimeSpan.FromMilliseconds(500)
                )
            );

            Assert.That(server.Requests, Is.EqualTo(1));
        }

        [
            Test,
            PuppeteerTest("httpUtil.test", "downloadFile", "rejects when all retries are exhausted")
        ]
        public void RejectsWhenAllRetriesAreExhausted()
        {
            using var server = new TestHttpServer(_ => 500);
            var destPath = Path.Combine(_tmpDir, "fail-download.bin");

            var ex = Assert.CatchAsync<HttpRequestException>(() =>
                Helpers.HttpUtil.DownloadFileAsync(server.Url, destPath, null, 2, 0)
            );

            Assert.That(ex.Message, Does.Contain("server returned code 500"));
            Assert.That(server.Requests, Is.EqualTo(3));
        }

        [
            Test,
            PuppeteerTest(
                "httpUtil.test",
                "downloadFile",
                "does not retry non-transient failures such as HTTP 404"
            )
        ]
        public void DoesNotRetryNonTransientFailuresSuchAsHTTP404()
        {
            using var server = new TestHttpServer(_ => 404);
            var destPath = Path.Combine(_tmpDir, "not-found-download.bin");

            var ex = Assert.CatchAsync<HttpRequestException>(() =>
                Helpers.HttpUtil.DownloadFileAsync(server.Url, destPath, null, 3, 0)
            );

            Assert.That(ex.Message, Does.Contain("server returned code 404"));
            Assert.That(server.Requests, Is.EqualTo(1));
        }

        [
            Test,
            PuppeteerTest(
                "httpUtil.test",
                "downloadFile",
                "does not retry when retry is omitted or zero"
            )
        ]
        public void DoesNotRetryWhenRetryIsOmittedOrZero()
        {
            using var server = new TestHttpServer(_ => 500);
            var destPath = Path.Combine(_tmpDir, "single-download.bin");

            Assert.CatchAsync<HttpRequestException>(() =>
                Helpers.HttpUtil.DownloadFileAsync(server.Url, destPath)
            );

            Assert.That(server.Requests, Is.EqualTo(1));
        }
    }
}
