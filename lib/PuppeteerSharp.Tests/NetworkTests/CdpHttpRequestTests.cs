using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class CdpHttpRequestTests
    {
        [Test]
        public void ShouldReconstructPostDataFromPostDataEntries()
        {
            var client = Substitute.For<CDPSession>();
            var frame = Substitute.For<IFrame>();
            var data = new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                LoaderId = "loaderId",
                Type = ResourceType.Document,
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostDataEntries = new[]
                    {
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part1")) },
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part2")) }
                    }
                }
            };

            var request = new CdpHttpRequest(
                client,
                frame,
                "interceptionId",
                true,
                data,
                new List<IRequest>(),
                NullLoggerFactory.Instance);

            Assert.That(request.PostData, Is.EqualTo("part1part2"));
        }

        [Test]
        public void ShouldFallbackToPostDataIfPostDataEntriesIsMissing()
        {
            var client = Substitute.For<CDPSession>();
            var frame = Substitute.For<IFrame>();
            var data = new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                LoaderId = "loaderId",
                Type = ResourceType.Document,
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostData = "originalData"
                }
            };

            var request = new CdpHttpRequest(
                client,
                frame,
                "interceptionId",
                true,
                data,
                new List<IRequest>(),
                NullLoggerFactory.Instance);

            Assert.That(request.PostData, Is.EqualTo("originalData"));
        }

        [Test]
        public void ShouldHandleMultiByteCharactersInPostDataEntries()
        {
            var client = Substitute.For<CDPSession>();
            var frame = Substitute.For<IFrame>();
            var multiByteString = "Hello 世界";
            var bytes = System.Text.Encoding.UTF8.GetBytes(multiByteString);

            // Split the bytes to simulate multi-byte characters split across entries
            var part1 = new byte[bytes.Length / 2];
            var part2 = new byte[bytes.Length - part1.Length];
            Array.Copy(bytes, 0, part1, 0, part1.Length);
            Array.Copy(bytes, part1.Length, part2, 0, part2.Length);

            var data = new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                LoaderId = "loaderId",
                Type = ResourceType.Document,
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostDataEntries = new[]
                    {
                        new PostDataEntry { Bytes = Convert.ToBase64String(part1) },
                        new PostDataEntry { Bytes = Convert.ToBase64String(part2) }
                    }
                }
            };

            var request = new CdpHttpRequest(
                client,
                frame,
                "interceptionId",
                true,
                data,
                new List<IRequest>(),
                NullLoggerFactory.Instance);

            Assert.That(request.PostData, Is.EqualTo(multiByteString));
        }

        [Test]
        public void ShouldHandleEmptyPostDataEntries()
        {
            var client = Substitute.For<CDPSession>();
            var frame = Substitute.For<IFrame>();
            var data = new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                LoaderId = "loaderId",
                Type = ResourceType.Document,
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostDataEntries = new PostDataEntry[0]
                }
            };

            var request = new CdpHttpRequest(
                client,
                frame,
                "interceptionId",
                true,
                data,
                new List<IRequest>(),
                NullLoggerFactory.Instance);

            Assert.That(request.PostData, Is.Null);
        }

        [Test]
        public void ShouldHandleNullBytesInPostDataEntries()
        {
            var client = Substitute.For<CDPSession>();
            var frame = Substitute.For<IFrame>();
            var data = new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                LoaderId = "loaderId",
                Type = ResourceType.Document,
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostDataEntries = new[]
                    {
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part1")) },
                        new PostDataEntry { Bytes = null },
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part2")) }
                    }
                }
            };

            var request = new CdpHttpRequest(
                client,
                frame,
                "interceptionId",
                true,
                data,
                new List<IRequest>(),
                NullLoggerFactory.Instance);

            Assert.That(request.PostData, Is.EqualTo("part1part2"));
        }
    }
}
