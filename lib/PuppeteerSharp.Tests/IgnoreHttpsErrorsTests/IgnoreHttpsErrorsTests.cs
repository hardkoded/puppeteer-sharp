using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    public class IgnoreHttpsErrorsTests : PuppeteerPageBaseTest
    {
        public IgnoreHttpsErrorsTests(): base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.IgnoreHTTPSErrors = true;
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "ignoreHTTPSErrors", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync($"{TestConstants.HttpsPrefix}/empty.html");
            Assert.True(response.Ok);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "ignoreHTTPSErrors", "should work with request interception")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWorkWithRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("ignorehttpserrors.spec.ts", "ignoreHTTPSErrors", "should work with mixed content")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
            Assert.AreEqual(2, Page.Frames.Length);
            // Make sure blocked iframe has functional execution context
            // @see https://github.com/GoogleChrome/puppeteer/issues/2709
            Assert.AreEqual(3, await Page.MainFrame.EvaluateExpressionAsync<int>("1 + 2"));
            Assert.AreEqual(5, await Page.FirstChildFrame().EvaluateExpressionAsync<int>("2 + 3"));
        }
    }
}
