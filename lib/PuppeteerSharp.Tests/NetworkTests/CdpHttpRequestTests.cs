using System;
using System.Collections.Generic;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests;

[TestFixture]
public class CdpHttpRequestTests
{
    [Test, PuppeteerTest("HTTPRequest.test.ts", "CdpHTTPRequest", "should reconstruct postData from postDataEntries")]
    public void ShouldReconstructPostDataFromPostDataEntries()
    {
        var client = Substitute.For<CDPSession>();
        var frame = Substitute.For<IFrame>();
        using var loggerFactory = new LoggerFactory();

        var request = new CdpHttpRequest(
            client,
            frame,
            "interceptionId",
            true,
            new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostDataEntries =
                    [
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part1")) },
                        new PostDataEntry { Bytes = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("part2")) },
                    ],
                },
                Initiator = new Initiator { Type = InitiatorType.Other },
            },
            [],
            loggerFactory);

        Assert.That(request.PostData, Is.EqualTo("part1part2"));
    }

    [Test, PuppeteerTest("HTTPRequest.test.ts", "CdpHTTPRequest", "should fallback to postData if postDataEntries is missing")]
    public void ShouldFallbackToPostDataIfPostDataEntriesIsMissing()
    {
        var client = Substitute.For<CDPSession>();
        var frame = Substitute.For<IFrame>();
        using var loggerFactory = new LoggerFactory();

        var request = new CdpHttpRequest(
            client,
            frame,
            "interceptionId",
            true,
            new RequestWillBeSentResponse
            {
                RequestId = "requestId",
                Request = new Request
                {
                    Url = "http://example.com",
                    Method = HttpMethod.Post,
                    Headers = new Dictionary<string, string>(),
                    PostData = "originalData",
                },
                Initiator = new Initiator { Type = InitiatorType.Other },
            },
            [],
            loggerFactory);

        Assert.That(request.PostData, Is.EqualTo("originalData"));
    }
}
