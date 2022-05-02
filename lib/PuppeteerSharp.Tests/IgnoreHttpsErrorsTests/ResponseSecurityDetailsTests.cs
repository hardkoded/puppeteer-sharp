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

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "should be |null| for non-secure requests")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldBeNullForNonSecureRequests()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Null(response.SecurityDetails);
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
    }
}
