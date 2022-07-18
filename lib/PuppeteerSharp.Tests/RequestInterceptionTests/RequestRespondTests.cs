using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class RequestRespondTests : DevToolsContextBaseTest
    {
        public RequestRespondTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.Created,
                    Headers = new Dictionary<string, object>
                    {
                        ["foo"] = "bar"
                    },
                    Body = "Yo, page!"
                });
            };

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.Created, response.Status);
            Assert.Equal("bar", response.Headers["foo"]);
            Assert.Equal("Yo, page!", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        /// <summary>
        /// In puppeteer this method is called ShouldWorkWithStatusCode422.
        /// I found that status 422 is not available in all .NET runtimes (see https://github.com/dotnet/core/blob/4c4642d548074b3fbfd425541a968aadd75fea99/release-notes/2.1/Preview/api-diff/preview2/2.1-preview2_System.Net.md)
        /// As the goal here is testing HTTP codes that are not in Chromium (see https://cs.chromium.org/chromium/src/net/http/http_status_code_list.h?sq=package:chromium&g=0) we will use code 426: Upgrade Required
        /// </summary>
        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should work with status code 422")]
        [PuppeteerFact]
        public async Task ShouldWorkReturnStatusPhrases()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.    Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.UpgradeRequired,
                    Body = "Yo, page!"
                });
            };

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.UpgradeRequired, response.Status);
            Assert.Equal("Upgrade Required", response.StatusText);
            Assert.Equal("Yo, page!", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should redirect")]
        [PuppeteerFact]
        public async Task ShouldRedirect()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);

            DevToolsContext.Request += async (_, e) =>
            {
                if (!e.Request.Url.Contains("rrredirect"))
                {
                    await e.Request.ContinueAsync();
                    return;
                }

                await e.Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.Redirect,
                    Headers = new Dictionary<string, object>
                    {
                        ["location"] = TestConstants.EmptyPage
                    }
                });
            };

            var response = await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/rrredirect");

            Assert.Single(response.Request.RedirectChain);
            Assert.Equal(TestConstants.ServerUrl + "/rrredirect", response.Request.RedirectChain[0].Url);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should allow mocking binary responses")]
        [PuppeteerFact]
        public async Task ShouldAllowMockingBinaryResponses()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                var imageData = System.IO.File.ReadAllBytes("./Assets/pptr.png");
                await e.Request.RespondAsync(new ResponseData
                {
                    ContentType = "image/png",
                    BodyData = imageData
                });
            };

            await DevToolsContext.EvaluateFunctionAsync(@"PREFIX =>
            {
                const img = document.createElement('img');
                img.src = PREFIX + '/does-not-exist.png';
                document.body.appendChild(img);
                return new Promise(fulfill => img.onload = fulfill);
            }", TestConstants.ServerUrl);
            var img = await DevToolsContext.QuerySelectorAsync("img");
            Assert.True(ScreenshotHelper.PixelMatch("mock-binary-response.png", await img.ScreenshotDataAsync()));
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should stringify intercepted request response headers")]
        [PuppeteerFact]
        public async Task ShouldStringifyInterceptedRequestResponseHeaders()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.OK,
                    Headers = new Dictionary<string, object>
                    {
                        ["foo"] = true
                    },
                    Body = "Yo, page!"
                });
            };

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal("True", response.Headers["foo"]);
            Assert.Equal("Yo, page!", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        [PuppeteerFact]
        public async Task ShouldAllowMultipleInterceptedRequestResponseHeaders()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) =>
            {
                await e.Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.OK,
                    Headers = new Dictionary<string, object>
                    {
                        ["foo"] = new bool[] { true, false },
                        ["Set-Cookie"] = new string[] { "sessionId=abcdef", "specialId=123456" }
                    },
                    Body = "Yo, page!"
                });
            };

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var cookies = await DevToolsContext.GetCookiesAsync(TestConstants.EmptyPage);

            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal("True\nFalse", response.Headers["foo"]);
            Assert.Equal("Yo, page!", await DevToolsContext.EvaluateExpressionAsync<string>("document.body.textContent"));
            Assert.Equal("specialId", cookies[0].Name);
            Assert.Equal("123456", cookies[0].Value);
            Assert.Equal("sessionId", cookies[1].Name);
            Assert.Equal("abcdef", cookies[1].Value);
        }
    }
}
