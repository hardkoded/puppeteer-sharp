using System.Collections.Generic;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests;

[TestFixture]
public class HeadersInputTests
{
    [Test, PuppeteerTest("HTTPRequest.test.ts", "HTTPRequest headersInputToRecord", "should throw when a header name contains CRLF characters")]
    public void ShouldThrowWhenAHeaderNameContainsCrlfCharacters()
    {
        var ex = Assert.Throws<PuppeteerException>(() =>
            HttpUtils.ValidateHeaders(new Dictionary<string, string> { ["x-foo\r\nx-injected"] = "bar" }));
        Assert.That(ex.Message, Does.Contain("CRLF"));
    }

    [Test, PuppeteerTest("HTTPRequest.test.ts", "HTTPRequest headersInputToRecord", "should throw when a header value contains CRLF characters")]
    public void ShouldThrowWhenAHeaderValueContainsCrlfCharacters()
    {
        var ex = Assert.Throws<PuppeteerException>(() =>
            HttpUtils.ValidateHeaders(new Dictionary<string, string> { ["x-foo"] = "bar\r\nx-injected: evil" }));
        Assert.That(ex.Message, Does.Contain("CRLF"));
    }
}
