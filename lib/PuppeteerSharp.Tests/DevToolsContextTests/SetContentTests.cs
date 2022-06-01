using System;
using System.Threading.Tasks;
using CefSharp;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetContentTests : DevToolsContextBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        public SetContentTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
        async Task Usage(IWebBrowser chromiumWebBrowser)
#pragma warning restore IDE0051 // Remove unused private members
        {
            #region SetContentAsync
            //Wait for Initial page load
            await chromiumWebBrowser.WaitForInitialLoadAsync();

            await using var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();
            await devtoolsContext.SetContentAsync("<div>My Receipt</div>");
            var result = await devtoolsContext.GetContentAsync();

            #endregion
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetContentAsync("<div>hello</div>");
            var result = await DevToolsContext.GetContentAsync();

            Assert.Equal(ExpectedOutput, result);
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with doctype")]
        [PuppeteerFact]
        public async Task ShouldWorkWithDoctype()
        {
            const string doctype = "<!DOCTYPE html>";

            await DevToolsContext.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await DevToolsContext.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with HTML 4 doctype")]
        [PuppeteerFact]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await DevToolsContext.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await DevToolsContext.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should respect timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await DevToolsContext.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>", new NavigationOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should respect default navigation timeout")]
        [PuppeteerFact]
        public async Task ShouldRespectDefaultTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            DevToolsContext.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await DevToolsContext.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>"));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should await resources to load")]
        [PuppeteerFact]
        public async Task ShouldAwaitResourcesToLoad()
        {
            var imgPath = "/img.png";
            var imgResponse = new TaskCompletionSource<bool>();
            Server.SetRoute(imgPath, _ => imgResponse.Task);
            var loaded = false;
            var waitTask = Server.WaitForRequest(imgPath);
            var contentTask = DevToolsContext.SetContentAsync($"<img src=\"{TestConstants.ServerUrl + imgPath}\"></img>")
                .ContinueWith(_ => loaded = true);
            await waitTask;
            Assert.False(loaded);
            imgResponse.SetResult(true);
            await contentTask;
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work fast enough")]
        [PuppeteerFact]
        public async Task ShouldWorkFastEnough()
        {
            for (var i = 0; i < 20; ++i)
            {
                await DevToolsContext.SetContentAsync("<div>yo</div>");
            }
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with tricky content")]
        [PuppeteerFact]
        public async Task ShouldWorkWithTrickyContent()
        {
            await DevToolsContext.SetContentAsync("<div>hello world</div>\x7F");
            Assert.Equal("hello world", await DevToolsContext.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with accents")]
        [PuppeteerFact]
        public async Task ShouldWorkWithAccents()
        {
            await DevToolsContext.SetContentAsync("<div>aberración</div>");
            Assert.Equal("aberración", await DevToolsContext.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with emojis")]
        [PuppeteerFact]
        public async Task ShouldWorkWithEmojis()
        {
            await DevToolsContext.SetContentAsync("<div>🐥</div>");
            Assert.Equal("🐥", await DevToolsContext.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [PuppeteerTest("page.spec.ts", "Page.setContent", "should work with newline")]
        [PuppeteerFact]
        public async Task ShouldWorkWithNewline()
        {
            await DevToolsContext.SetContentAsync("<div>\n</div>");
            Assert.Equal("\n", await DevToolsContext.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }
    }
}
