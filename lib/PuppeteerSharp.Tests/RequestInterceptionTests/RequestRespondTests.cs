using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.RequestInterceptionTests
{
    public class RequestRespondTests : PuppeteerPageBaseTest
    {
        public RequestRespondTests(): base()
        {
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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
        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should work with status code 422")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkReturnStatusPhrases()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                await e.    Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.UpgradeRequired,
                    Body = "Yo, page!"
                });
            };

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.UpgradeRequired, response.Status);
            Assert.AreEqual("Upgrade Required", response.StatusText);
            Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should redirect")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldRedirect()
        {
            await Page.SetRequestInterceptionAsync(true);

            Page.Request += async (_, e) =>
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

            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/rrredirect");

            Assert.That(response.Request.RedirectChain, Has.Exactly(1).Items);
            Assert.AreEqual(TestConstants.ServerUrl + "/rrredirect", response.Request.RedirectChain[0].Url);
            Assert.AreEqual(TestConstants.EmptyPage, response.Url);
        }

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should allow mocking binary responses")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAllowMockingBinaryResponses()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
            {
                var imageData = File.ReadAllBytes("./Assets/pptr.png");
                await e.Request.RespondAsync(new ResponseData
                {
                    ContentType = "image/png",
                    BodyData = imageData
                });
            };

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

        [PuppeteerTest("requestinterception.spec.ts", "Request.respond", "should stringify intercepted request response headers")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldStringifyInterceptedRequestResponseHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.AreEqual("True", response.Headers["foo"]);
            Assert.AreEqual("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldAllowMultipleInterceptedRequestResponseHeaders()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) =>
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
}
