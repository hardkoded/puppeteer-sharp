using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using CefSharp.DevTools.Dom.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Microsoft.AspNetCore.Connections.Features;
using CefSharp.DevTools.Dom;
using Microsoft.AspNetCore.Http;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ResponseSecurityDetailsTests : DevToolsContextBaseTest
    {
        public ResponseSecurityDetailsTests(ITestOutputHelper output) : base(output)
        {
            
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "Should Work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var requestTask = HttpsServer.WaitForRequest(
                "/empty.html",
                request => request?.HttpContext?.Features?.Get<ITlsHandshakeFeature>()?.Protocol);
            var responseTask = DevToolsContext.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");

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
        [PuppeteerFact(Skip ="TODO: CEF")]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var responses = new List<Response>();
            HttpsServer.SetRedirect("/plzredirect", "/empty.html");

            DevToolsContext.Response += (_, e) => responses.Add(e.Response);

            var requestTask = HttpsServer.WaitForRequest(
                "/empty.html",
                request => request?.HttpContext?.Features?.Get<ITlsHandshakeFeature>()?.Protocol);
            var responseTask = DevToolsContext.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");

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
        [PuppeteerFact]
        public async Task ShouldWorkWithRequestInterception()
        {
            await DevToolsContext.SetRequestInterceptionAsync(true);
            DevToolsContext.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "should work with mixed content")]
        [PuppeteerFact(Skip = "BUG: OOPIFs aren't working correct")]
        public async Task ShouldWorkWithMixedContent()
        {
            HttpsServer.SetRoute("/mixedcontent.html", async (context) =>
            {
                await context.Response.WriteAsync($"<iframe src='{TestConstants.EmptyPage}'></iframe>");
            });
            await DevToolsContext.GoToAsync(TestConstants.HttpsPrefix + "/mixedcontent.html", new NavigationOptions
            {
                WaitUntil = new[] { WaitUntilNavigation.Load }
            });
            Assert.Equal(2, DevToolsContext.Frames.Length);
            // Make sure blocked iframe has functional execution context
            // @see https://github.com/GoogleChrome/puppeteer/issues/2709
            Assert.Equal(3, await DevToolsContext.MainFrame.EvaluateExpressionAsync<int>("1 + 2"));
            Assert.Equal(5, await DevToolsContext.FirstChildFrame().EvaluateExpressionAsync<int>("2 + 3"));
        }
    }
}
