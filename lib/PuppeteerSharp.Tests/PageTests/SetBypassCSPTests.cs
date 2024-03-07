using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetBypassCSPTests : PuppeteerPageBaseTest
    {
        public SetBypassCSPTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setBypassCSP", "should bypass CSP meta tag")]
        public async Task ShouldBypassCSPMetaTag()
        {
            // Make sure CSP prohibits addScriptTag.
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await Page.EvaluateExpressionAsync("window.__injected"));

            // By-pass CSP and try one more time.
            await Page.SetBypassCSPAsync(true);
            await Page.ReloadAsync();
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setBypassCSP", "should bypass CSP header")]
        public async Task ShouldBypassCSPHeader()
        {
            // Make sure CSP prohibits addScriptTag.
            Server.SetCSP("/empty.html", "default-src 'self'");
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await Page.EvaluateExpressionAsync("window.__injected"));

            // By-pass CSP and try one more time.
            await Page.SetBypassCSPAsync(true);
            await Page.ReloadAsync();
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setBypassCSP", "should bypass after cross-process navigation")]
        public async Task ShouldBypassAfterCrossProcessNavigation()
        {
            await Page.SetBypassCSPAsync(true);
            await Page.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("window.__injected"));

            await Page.GoToAsync(TestConstants.CrossProcessUrl + "/csp.html");
            await Page.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.AreEqual(42, await Page.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setBypassCSP", "should bypass CSP in iframes as well")]
        public async Task ShouldBypassCSPInIframesAsWell()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);

            // Make sure CSP prohibits addScriptTag in an iframe.
            var frame = await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.ServerUrl + "/csp.html");
            await frame.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await frame.EvaluateFunctionAsync<int?>("() => window.__injected"));

            // By-pass CSP and try one more time.
            await Page.SetBypassCSPAsync(true);
            await Page.ReloadAsync();

            // Make sure CSP prohibits addScriptTag in an iframe.
            frame = await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.ServerUrl + "/csp.html");
            await frame.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.AreEqual(42, await frame.EvaluateFunctionAsync<int?>("() => window.__injected"));
        }
    }
}
