using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ResponseSecurityDetailsTests : PuppeteerPageBaseTest
    {
        public ResponseSecurityDetailsTests(ITestOutputHelper output) : base(output)
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.IgnoreHTTPSErrors = true;
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "Should Work")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWork()
        {
            var requestTask = HttpsServer.WaitForRequest(
                "/empty.html",
                request => request?.HttpContext?.Features?.Get<ITlsHandshakeFeature>()?.Protocol);
            var responseTask = Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

            await Task.WhenAll(
                requestTask,
                responseTask).WithTimeout();

            var response = responseTask.Result;
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.NotNull(response.SecurityDetails);
            Assert.Equal(
                TestUtils.CurateProtocol(requestTask.Result.ToString()),
                TestUtils.CurateProtocol(response.SecurityDetails.Protocol));
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "Network redirects should report SecurityDetails")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var responses = new List<Response>();
            HttpsServer.SetRedirect("/plzredirect", "/empty.html");

            Page.Response += (_, e) => responses.Add(e.Response);

            var requestTask = HttpsServer.WaitForRequest(
                "/empty.html",
                request => request?.HttpContext?.Features?.Get<ITlsHandshakeFeature>()?.Protocol);
            var responseTask = Page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");

            await Task.WhenAll(
                requestTask,
                responseTask).WithTimeout();

            var response = responseTask.Result;

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.Found, responses[0].Status);
            Assert.Equal(
                TestUtils.CurateProtocol(requestTask.Result.ToString()),
                TestUtils.CurateProtocol(response.SecurityDetails.Protocol));
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "should work with request interception")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "should work with mixed content")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldWorkWithMixedContent()
        {
            HttpsServer.SetRoute("/mixedcontent.html", async (context) =>
            {
                await context.Response.WriteAsync($"<iframe src='{TestConstants.EmptyPage}'></iframe>");
            });
            await Page.GoToAsync(TestConstants.HttpsPrefix + "/mixedcontent.html", new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Load }
            });
            Assert.Equal(2, Page.Frames.Length);
            // Make sure blocked iframe has functional execution context
            // @see https://github.com/GoogleChrome/puppeteer/issues/2709
            Assert.Equal(3, await Page.MainFrame.EvaluateExpressionAsync<int>("1 + 2"));
            Assert.Equal(5, await Page.FirstChildFrame().EvaluateExpressionAsync<int>("2 + 3"));
        }
    }
}
