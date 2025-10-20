using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    public class ResponseSecurityDetailsTests : PuppeteerPageBaseTest
    {
        public ResponseSecurityDetailsTests() : base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.AcceptInsecureCerts = true;
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "Response.securityDetails", "Should Work")]
        public async Task ShouldWork()
        {
            // Checking for the TLS socket is it is in upstreams proves to be flaky in .net framework.
            // We don't need to test that here.

            var response = await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response.SecurityDetails, Is.Not.Null);
            Assert.That(response.SecurityDetails.Protocol, Does.Contain("TLS"));
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "Response.securityDetails", "should be |null| for non-secure requests")]
        public async Task ShouldBeNullForNonSecureRequests()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.SecurityDetails, Is.Null);
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "Response.securityDetails", "Network redirects should report SecurityDetails")]
        [Ignore("This is super flaky")]
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

            Assert.That(responses, Has.Count.EqualTo(2));
            Assert.That(responses[0].Status, Is.EqualTo(HttpStatusCode.Found));
            Assert.That(TestUtils.CurateProtocol(response.SecurityDetails.Protocol),
                Is.EqualTo(TestUtils.CurateProtocol(requestTask.Result.ToString())));
        }
    }
}
