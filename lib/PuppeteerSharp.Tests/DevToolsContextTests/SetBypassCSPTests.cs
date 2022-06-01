using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetBypassCSPTests : DevToolsContextBaseTest
    {
        public SetBypassCSPTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setBypassCSP", "should bypass CSP meta tag")]
        [PuppeteerFact]
        public async Task ShouldBypassCSPMetaTag()
        {
            // Make sure CSP prohibits addScriptTag.
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await DevToolsContext.EvaluateExpressionAsync("window.__injected"));

            // By-pass CSP and try one more time.
            await DevToolsContext.SetBypassCSPAsync(true);
            await DevToolsContext.ReloadAsync();
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setBypassCSP", "should bypass CSP header")]
        [PuppeteerFact]
        public async Task ShouldBypassCSPHeader()
        {
            // Make sure CSP prohibits addScriptTag.
            Server.SetCSP("/empty.html", "default-src 'self'");
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await DevToolsContext.EvaluateExpressionAsync("window.__injected"));

            // By-pass CSP and try one more time.
            await DevToolsContext.SetBypassCSPAsync(true);
            await DevToolsContext.ReloadAsync();
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setBypassCSP", "should bypass after cross-process navigation")]
        [PuppeteerFact]
        public async Task ShouldBypassAfterCrossProcessNavigation()
        {
            await DevToolsContext.SetBypassCSPAsync(true);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/csp.html");
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("window.__injected"));

            await DevToolsContext.GoToAsync(TestConstants.CrossProcessUrl + "/csp.html");
            await DevToolsContext.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            });
            Assert.Equal(42, await DevToolsContext.EvaluateExpressionAsync<int>("window.__injected"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setBypassCSP", "should bypass CSP in iframes as well")]
        [PuppeteerFact]
        public async Task ShouldBypassCSPInIframesAsWell()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);

            // Make sure CSP prohibits addScriptTag in an iframe.
            var frame = await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.ServerUrl + "/csp.html");
            await frame.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Null(await frame.EvaluateFunctionAsync<int?>("() => window.__injected"));

            // By-pass CSP and try one more time.
            await DevToolsContext.SetBypassCSPAsync(true);
            await DevToolsContext.ReloadAsync();

            // Make sure CSP prohibits addScriptTag in an iframe.
            frame = await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.ServerUrl + "/csp.html");
            await frame.AddScriptTagAsync(new AddTagOptions
            {
                Content = "window.__injected = 42;"
            }).ContinueWith(_ => Task.CompletedTask);
            Assert.Equal(42, await frame.EvaluateFunctionAsync<int?>("() => window.__injected"));
        }
    }
}
