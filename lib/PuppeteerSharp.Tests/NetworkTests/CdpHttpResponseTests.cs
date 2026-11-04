using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using PuppeteerSharp.Cdp;
using PuppeteerSharp.Cdp.Messaging;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests;

[TestFixture]
public class CdpHttpResponseTests
{
    [Test, PuppeteerTest("HTTPResponse.test.ts", "CdpHTTPResponse", "should normalize set-cookie using \\n")]
    public void ShouldNormalizeSetCookieUsingNewline()
    {
        var request = CreateRequest();
        var responsePayload = new ResponsePayload
        {
            RemoteIPAddress = "127.0.0.1",
            RemotePort = 80,
            Status = HttpStatusCode.OK,
            StatusText = "OK",
            Headers = new Dictionary<string, string>
            {
                ["set-cookie"] = "a=b\n  c=d",
            },
        };

        var response = new CdpHttpResponse(request, responsePayload, null);

        Assert.That(response.Headers["set-cookie"], Is.EqualTo("a=b\n c=d"));
    }

    [Test, PuppeteerTest("HTTPResponse.test.ts", "CdpHTTPResponse", "should normalize other headers using ,")]
    public void ShouldNormalizeOtherHeadersUsingComma()
    {
        var request = CreateRequest();
        var responsePayload = new ResponsePayload
        {
            RemoteIPAddress = "127.0.0.1",
            RemotePort = 80,
            Status = HttpStatusCode.OK,
            StatusText = "OK",
            Headers = new Dictionary<string, string>
            {
                ["content-type"] = "text/html\n  charset=utf-8",
                ["accept-language"] = "en-US\n en",
            },
        };

        var response = new CdpHttpResponse(request, responsePayload, null);

        Assert.That(response.Headers["content-type"], Is.EqualTo("text/html, charset=utf-8"));
        Assert.That(response.Headers["accept-language"], Is.EqualTo("en-US, en"));
    }

    private static CdpHttpRequest CreateRequest()
    {
        var client = Substitute.For<CDPSession>();
        var frame = Substitute.For<IFrame>();
        using var loggerFactory = new LoggerFactory();

        return new CdpHttpRequest(
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
                    Method = HttpMethod.Get,
                    Headers = new Dictionary<string, string>(),
                },
                Initiator = new Initiator { Type = InitiatorType.Other },
            },
            [],
            loggerFactory);
    }
}
