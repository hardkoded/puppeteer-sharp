using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Locators;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorRaceTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator.race", "races multiple locators")]
        public async Task RacesMultipleLocators()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync("<button onclick=\"window.count++;\">test</button>");
            await Page.EvaluateExpressionAsync("window.count = 0");

            await Locator.Race(
                Page.Locator("button"),
                Page.Locator("button")).ClickAsync();

            var count = await Page.EvaluateExpressionAsync<int>("globalThis.count");
            Assert.That(count, Is.EqualTo(1));
        }

        [Test, PuppeteerTest("locator.spec", "Locator.race", "can be aborted")]
        public async Task CanBeAborted()
        {
            await Page.SetViewportAsync(new ViewPortOptions { Width = 500, Height = 500 });
            await Page.SetContentAsync(
                "<button style=\"display: none;\" onclick=\"this.innerText = 'clicked';\">test</button>");

            using var cts = new CancellationTokenSource();
            var resultTask = Locator.Race(
                    Page.Locator("button"),
                    Page.Locator("button"))
                .SetTimeout(5000)
                .ClickAsync(new LocatorClickOptions { CancellationToken = cts.Token });

            await Task.Delay(2000);
            cts.Cancel();

            var exception = Assert.CatchAsync(async () => await resultTask);
            Assert.That(exception, Is.InstanceOf<OperationCanceledException>());
        }

        [Test, PuppeteerTest("locator.spec", "Locator.race", "should time out when all locators do not match")]
        public async Task ShouldTimeOutWhenAllLocatorsDoNotMatch()
        {
            await Page.SetContentAsync("<button>test</button>");

            var exception = Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await Locator.Race(
                        Page.Locator("not-found"),
                        Page.Locator("not-found"))
                    .SetTimeout(5000)
                    .ClickAsync();
            });

            Assert.That(exception.Message, Does.Contain("Timed out after waiting 5000ms"));
        }

        [Test, PuppeteerTest("locator.spec", "Locator.race", "should not time out when one of the locators matches")]
        public async Task ShouldNotTimeOutWhenOneOfTheLocatorsMatches()
        {
            await Page.SetContentAsync("<button>test</button>");

            await Locator.Race(
                Page.Locator("not-found"),
                Page.Locator("button")).ClickAsync();
        }
    }
}
