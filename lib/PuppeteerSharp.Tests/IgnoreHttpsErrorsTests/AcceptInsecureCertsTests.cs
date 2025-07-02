using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.IgnoreHttpsErrorsTests
{
    public class AcceptInsecureCertsTests : PuppeteerPageBaseTest
    {
        public AcceptInsecureCertsTests() : base()
        {
            DefaultOptions = TestConstants.DefaultBrowserOptions();
            DefaultOptions.AcceptInsecureCerts = true;
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "ignoreHTTPSErrors", "should work")]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync($"{TestConstants.HttpsPrefix}/empty.html");
            Assert.That(response.Ok, Is.True);
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "ignoreHTTPSErrors", "should work with request interception")]
        public async Task ShouldWorkWithRequestInterception()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (_, e) => await e.Request.ContinueAsync();
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("acceptInsecureCerts.spec", "ignoreHTTPSErrors", "should work with mixed content")]
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
            Assert.That(Page.Frames, Has.Length.EqualTo(2));
            // Make sure blocked iframe has functional execution context
            // @see https://github.com/GoogleChrome/puppeteer/issues/2709
            Assert.That(await Page.MainFrame.EvaluateExpressionAsync<int>("1 + 2"), Is.EqualTo(3));
            Assert.That(await Page.FirstChildFrame().EvaluateExpressionAsync<int>("2 + 3"), Is.EqualTo(5));
        }
    }
}
