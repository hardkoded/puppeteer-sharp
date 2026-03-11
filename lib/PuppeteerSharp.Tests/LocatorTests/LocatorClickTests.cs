using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Locators;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorClickTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button onclick=\"this.innerText = 'clicked';\">test</button>");

            await Page.Locator("button").ClickAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work for multiple selectors")]
        public async Task ShouldWorkForMultipleSelectors()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button onclick=\"this.innerText = 'clicked';\">test</button>");

            await Page.Locator("::-p-text(test), ::-p-xpath(/button)").ClickAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work if the element is out of viewport")]
        public async Task ShouldWorkIfTheElementIsOutOfViewport()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"margin-top: 600px;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            await Page.Locator("button").ClickAsync();

            var button = await Page.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work with element handles")]
        public async Task ShouldWorkWithElementHandles()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"margin-top: 600px;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            var button = await Page.QuerySelectorAsync("button");
            Assert.That(button, Is.Not.Null);

            await button.AsLocator().ClickAsync();

            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work if the element becomes visible later")]
        public async Task ShouldWorkIfTheElementBecomesVisibleLater()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"display: none;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            var button = await Page.QuerySelectorAsync("button");
            var resultTask = Page.Locator("button").ClickAsync();

            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("test"));

            await button.EvaluateFunctionAsync("el => el.style.display = 'block'");
            await resultTask;

            text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work if the element becomes enabled later")]
        public async Task ShouldWorkIfTheElementBecomesEnabledLater()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button disabled onclick=\"this.innerText = 'clicked';\">test</button>");

            var button = await Page.QuerySelectorAsync("button");
            var resultTask = Page.Locator("button").ClickAsync();

            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("test"));

            await button.EvaluateFunctionAsync("el => el.disabled = false");
            await resultTask;

            text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work if multiple conditions are satisfied later")]
        public async Task ShouldWorkIfMultipleConditionsAreSatisfiedLater()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"margin-top: 600px;\" style=\"display: none;\" disabled onclick=\"this.innerText = 'clicked';\">test</button>");

            var button = await Page.QuerySelectorAsync("button");
            var resultTask = Page.Locator("button").ClickAsync();

            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("test"));

            await button.EvaluateFunctionAsync(@"el => {
                el.disabled = false;
                el.style.display = 'block';
            }");
            await resultTask;

            text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should time out")]
        public async Task ShouldTimeOut()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"display: none;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await Page.Locator("button").SetTimeout(5000).ClickAsync();
            });

            Assert.That(exception.Message, Does.Contain("Timed out after waiting 5000ms"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should retry clicks on errors")]
        public async Task ShouldRetryClicksOnErrors()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"display: none;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await Page.Locator("button").SetTimeout(5000).ClickAsync();
            });

            Assert.That(exception.Message, Does.Contain("Timed out after waiting 5000ms"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "can be aborted")]
        public async Task CanBeAborted()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"display: none;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            using var cts = new CancellationTokenSource();
            var resultTask = Page.Locator("button").SetTimeout(5000).ClickAsync(new LocatorClickOptions
            {
                CancellationToken = cts.Token,
            });

            await Task.Delay(2000);
            cts.Cancel();

            var exception = Assert.CatchAsync(async () => await resultTask);
            Assert.That(exception, Is.InstanceOf<OperationCanceledException>());
        }

        [Test, PuppeteerTest("locator.spec", "Locator Locator.click", "should work with a OOPIF")]
        public async Task ShouldWorkWithAOOPIF()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<iframe src=\"data:text/html,<button onclick=&quot;this.innerText = 'clicked';&quot;>test</button>\"></iframe>");

            var frame = await Page.WaitForFrameAsync(f => f.Url.StartsWith("data"));

            await frame.Locator("button").ClickAsync();

            var button = await frame.QuerySelectorAsync("button");
            var text = await button.EvaluateFunctionAsync<string>("el => el.innerText");
            Assert.That(text, Is.EqualTo("clicked"));
        }
    }
}
