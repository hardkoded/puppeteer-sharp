using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.RequestInterceptionExperimentalTests;

public class RequestRespondTests : PuppeteerPageBaseTest
{
    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Request.respond", "should work")]
    public async Task ShouldWork()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.RespondAsync(new ResponseData
        {
            Status = HttpStatusCode.Created,
            Headers = new Dictionary<string, object> { ["foo"] = "bar" },
            Body = "Yo, page!"
        }, 0));

        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.AreEqual(HttpStatusCode.Created, response.Status);
        Assert.AreEqual("bar", response.Headers["foo"]);
        Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
    }

    /// <summary>
    /// In puppeteer this method is called ShouldWorkWithStatusCode422.
    /// I found that status 422 is not available in all .NET runtimes (see https://github.com/dotnet/core/blob/4c4642d548074b3fbfd425541a968aadd75fea99/release-notes/2.1/Preview/api-diff/preview2/2.1-preview2_System.Net.md)
    /// As the goal here is testing HTTP codes that are not in Chromium (see https://cs.chromium.org/chromium/src/net/http/http_status_code_list.h?sq=package:chromium&g=0) we will use code 426: Upgrade Required
    /// </summary>
    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Request.respond", "should work with status code 422")]
    public async Task ShouldWorkReturnStatusPhrases()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.RespondAsync(new ResponseData
        {
            Status = HttpStatusCode.UpgradeRequired,
            Body = "Yo, page!"
        }, 0));

        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.AreEqual(HttpStatusCode.UpgradeRequired, response.Status);
        Assert.AreEqual("Upgrade Required", response.StatusText);
        Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Request.respond", "should redirect")]
    public async Task ShouldRedirect()
    {
        await Page.SetRequestInterceptionAsync(true);

        Page.AddRequestInterceptor(request =>
        {
            if (!request.Url.Contains("rrredirect"))
            {
                return request.ContinueAsync(new Payload(), 0);
            }

            return request.RespondAsync(new ResponseData
            {
                Status = HttpStatusCode.Redirect,
                Headers = new Dictionary<string, object> { ["location"] = TestConstants.EmptyPage }
            }, 0);
        });

        var response = await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");

        Assert.That(response.Request.RedirectChain, Has.Exactly(1).Items);
        Assert.AreEqual(TestConstants.ServerUrl + "/rrredirect", response.Request.RedirectChain[0].Url);
        Assert.AreEqual(TestConstants.EmptyPage, response.Url);
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Request.respond", "should allow mocking binary responses")]
    public async Task ShouldAllowMockingBinaryResponses()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            var imageData = File.ReadAllBytes("./Assets/pptr.png");
            return request.RespondAsync(new ResponseData { ContentType = "image/png", BodyData = imageData }, 0);
        });

        await Page.EvaluateFunctionAsync(@"PREFIX =>
            {
                const img = document.createElement('img');
                img.src = PREFIX + '/does-not-exist.png';
                document.body.appendChild(img);
                return new Promise(fulfill => img.onload = fulfill);
            }", TestConstants.ServerUrl);
        var img = await Page.QuerySelectorAsync("img");
        Assert.True(ScreenshotHelper.PixelMatch("mock-binary-response.png", await img.ScreenshotDataAsync()));
    }

    [Test, Retry(2), PuppeteerTest("requestinterception-experimental.spec", "Request.respond",
        "should stringify intercepted request response headers")]
    public async Task ShouldStringifyInterceptedRequestResponseHeaders()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request => request.RespondAsync(new ResponseData
        {
            Status = HttpStatusCode.OK,
            Headers = new Dictionary<string, object> { ["foo"] = true },
            Body = "Yo, page!"
        }, 0));

        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        Assert.AreEqual(HttpStatusCode.OK, response.Status);
        Assert.AreEqual("True", response.Headers["foo"]);
        Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
    }

    public async Task ShouldAllowMultipleInterceptedRequestResponseHeaders()
    {
        await Page.SetRequestInterceptionAsync(true);
        Page.AddRequestInterceptor(request =>
        {
            return request.RespondAsync(new ResponseData
            {
                Status = HttpStatusCode.OK,
                Headers = new Dictionary<string, object>
                {
                    ["foo"] = new[] { true, false },
                    ["Set-Cookie"] = new[] { "sessionId=abcdef", "specialId=123456" }
                },
                Body = "Yo, page!"
            }, 0);
        });

        var response = await Page.GoToAsync(TestConstants.EmptyPage);
        var cookies = await Page.GetCookiesAsync(TestConstants.EmptyPage);

        Assert.AreEqual(HttpStatusCode.OK, response.Status);
        Assert.AreEqual("True\nFalse", response.Headers["foo"]);
        Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
        Assert.AreEqual("specialId", cookies[0].Name);
        Assert.AreEqual("123456", cookies[0].Value);
        Assert.AreEqual("sessionId", cookies[1].Name);
        Assert.AreEqual("abcdef", cookies[1].Value);
    }
}
