using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Helpers;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PuppeteerTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class IgnoreHttpsErrorsTests : PuppeteerPageBaseTest
    {
        public IgnoreHttpsErrorsTests(ITestOutputHelper output) : base(output)
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.IgnoreHTTPSErrors = true;
        }

        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.NotNull(response.SecurityDetails);
            Assert.Equal("TLS 1.2", response.SecurityDetails.Protocol);
        }

        [Fact]
        public async Task NetworkRedirectsShouldReportSecurityDetails()
        {
            var responses = new List<Response>();
            HttpsServer.SetRedirect("/plzredirect", "/empty.html");

            Page.Response += (sender, e) => responses.Add(e.Response);

            await Page.GoToAsync(TestConstants.HttpsPrefix + "/plzredirect");

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.Found, responses[0].Status);
            Assert.Equal("TLS 1.2", responses[0].SecurityDetails.Protocol);
        }

        [Fact]
        public async Task ShouldWorkWithRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
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