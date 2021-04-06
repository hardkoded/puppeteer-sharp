using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetContentTests : PuppeteerPageBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        public SetContentTests(ITestOutputHelper output) : base(output)
        {
        }

        async Task Usage(Browser browser)
        {
            #region SetContentAsync

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync("<div>My Receipt</div>");
            var result = await page.GetContentAsync();

            #endregion
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal(ExpectedOutput, result);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithDoctype()
        {
            const string doctype = "<!DOCTYPE html>";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldRespectTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>", new NavigationOptions
                {
                    Timeout = 1
                }));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldRespectDefaultTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<TimeoutException>(async () =>
                await Page.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>"));

            Assert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldAwaitResourcesToLoad()
        {
            var imgPath = "/img.png";
            var imgResponse = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Server.SetRoute(imgPath, _ => imgResponse.Task);
            var loaded = false;
            var waitTask = Server.WaitForRequest(imgPath);
            var contentTask = Page.SetContentAsync($"<img src=\"{TestConstants.ServerUrl + imgPath}\"></img>")
                .ContinueWith(_ => loaded = true);
            await waitTask;
            Assert.False(loaded);
            imgResponse.SetResult(true);
            await contentTask;
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkFastEnough()
        {
            for (var i = 0; i < 20; ++i)
            {
                await Page.SetContentAsync("<div>yo</div>");
            }
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithTrickyContent()
        {
            await Page.SetContentAsync("<div>hello world</div>\x7F");
            Assert.Equal("hello world", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithAccents()
        {
            await Page.SetContentAsync("<div>aberración</div>");
            Assert.Equal("aberración", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithEmojis()
        {
            await Page.SetContentAsync("<div>🐥</div>");
            Assert.Equal("🐥", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkWithNewline()
        {
            await Page.SetContentAsync("<div>\n</div>");
            Assert.Equal("\n", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }
    }
}
