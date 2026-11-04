using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using PuppeteerSharp.Bidi;
using PuppeteerSharp.Nunit;
using WebDriverBiDi.JsonConverters;

namespace PuppeteerSharp.Tests.NetworkTests;

[TestFixture]
public class BidiHttpResponseTests
{
    [Test, PuppeteerTest("HTTPResponse.test.ts", "BidiHTTPResponse", "should combine duplicate set-cookie headers using \\n")]
    public void ShouldCombineDuplicateSetCookieHeadersUsingNewline()
    {
        var data = CreateResponseData(("set-cookie", "a=b"), ("set-cookie", "c=d"));

        var response = BidiHttpResponse.FromResponseData(data);

        Assert.That(response.Headers["set-cookie"], Is.EqualTo("a=b\n c=d"));
    }

    [Test, PuppeteerTest("HTTPResponse.test.ts", "BidiHTTPResponse", "should combine other duplicate headers using ,")]
    public void ShouldCombineOtherDuplicateHeadersUsingComma()
    {
        var data = CreateResponseData(("cache-control", "no-cache"), ("cache-control", "no-store"));

        var response = BidiHttpResponse.FromResponseData(data);

        Assert.That(response.Headers["cache-control"], Is.EqualTo("no-cache, no-store"));
    }

    private static WebDriverBiDi.Network.ResponseData CreateResponseData(params (string Name, string Value)[] headers)
    {
        var headersJson = string.Join(
            ",",
            headers.Select(header => $"{{\"name\":\"{header.Name}\",\"value\":{{\"type\":\"string\",\"value\":\"{header.Value}\"}}}}"));

        var json = "{"
            + "\"url\":\"http://localhost/\","
            + "\"protocol\":\"http/1.1\","
            + "\"status\":200,"
            + "\"statusText\":\"OK\","
            + "\"fromCache\":false,"
            + "\"mimeType\":\"text/plain\","
            + "\"bytesReceived\":0,"
            + "\"headersSize\":0,"
            + "\"bodySize\":0,"
            + "\"content\":{\"size\":0},"
            + $"\"headers\":[{headersJson}]"
            + "}";

        return JsonSerializer.Deserialize(json, WebDriverBiDiJsonSerializerContext.Default.ResponseData);
    }
}
