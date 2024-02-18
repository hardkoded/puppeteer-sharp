using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetContentTests : PuppeteerPageBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        public async Task Usage(IBrowser browser)
        {
            #region SetContentAsync

            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync("<div>My Receipt</div>");
            var result = await page.GetContentAsync();

            #endregion
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.AreEqual(ExpectedOutput, result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with doctype")]
        public async Task ShouldWorkWithDoctype()
        {
            const string doctype = "<!DOCTYPE html>";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.AreEqual($"{doctype}{ExpectedOutput}", result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with HTML 4 doctype")]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.AreEqual($"{doctype}{ExpectedOutput}", result);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should respect timeout")]
        public async Task ShouldRespectTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await Page.GoToAsync(TestConstants.EmptyPage);
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>", new NavigationOptions
                {
                    Timeout = 1
                }));

            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should respect default navigation timeout")]
        public async Task ShouldRespectDefaultTimeout()
        {
            const string imgPath = "/img.png";
            Server.SetRoute(imgPath, _ => Task.Delay(-1));

            await Page.GoToAsync(TestConstants.EmptyPage);
            Page.DefaultTimeout = 1;
            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
                await Page.SetContentAsync($"<img src='{TestConstants.ServerUrl + imgPath}'></img>"));

            StringAssert.Contains("Timeout of 1 ms exceeded", exception.Message);
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should await resources to load")]
        public async Task ShouldAwaitResourcesToLoad()
        {
            var imgPath = "/img.png";
            var imgResponse = new TaskCompletionSource<bool>();
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

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work fast enough")]
        public async Task ShouldWorkFastEnough()
        {
            for (var i = 0; i < 20; ++i)
            {
                await Page.SetContentAsync("<div>yo</div>");
            }
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with tricky content")]
        public async Task ShouldWorkWithTrickyContent()
        {
            await Page.SetContentAsync("<div>hello world</div>\x7F");
            Assert.AreEqual("hello world", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with accents")]
        public async Task ShouldWorkWithAccents()
        {
            await Page.SetContentAsync("<div>aberración</div>");
            Assert.AreEqual("aberración", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with emojis")]
        public async Task ShouldWorkWithEmojis()
        {
            await Page.SetContentAsync("<div>🐥</div>");
            Assert.AreEqual("🐥", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setContent", "should work with newline")]
        public async Task ShouldWorkWithNewline()
        {
            await Page.SetContentAsync("<div>\n</div>");
            Assert.AreEqual("\n", await Page.QuerySelectorAsync("div").EvaluateFunctionAsync<string>("div => div.textContent"));
        }
    }
}
