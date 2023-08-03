using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    public class ResponseSecurityDetailsTests : PuppeteerPageBaseTest
    {
        public ResponseSecurityDetailsTests(): base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.IgnoreHTTPSErrors = true;
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "Should Work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            // Checking for the TLS socket is it is in upstreams proves to be flacky in .net framework.
            // We don't need to test that here.

            var response = await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
            Assert.NotNull(response.SecurityDetails);
            Assert.Contains("TLS", response.SecurityDetails.Protocol);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "should be |null| for non-secure requests")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeNullForNonSecureRequests()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Null(response.SecurityDetails);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "Response.securityDetails", "Network redirects should report SecurityDetails")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var responses = new List<IResponse>();
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

            Assert.AreEqual(2, responses.Count);
            Assert.AreEqual(HttpStatusCode.Found, responses[0].Status);
            Assert.AreEqual(
                TestUtils.CurateProtocol(requestTask.Result.ToString()),
                TestUtils.CurateProtocol(response.SecurityDetails.Protocol));
        }
    }
}
