using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TargetTests
{
    public class BrowserWaitForTargetTests : PuppeteerPageBaseTest
    {
        public BrowserWaitForTargetTests() : base()
        {
        }

        [Test, PuppeteerTest("target.spec", "Target Browser.waitForTarget", "should wait for a target")]
        public async Task ShouldWaitForATarget()
        {
            var targetTask = Browser.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage, new() { Timeout = 3000 });
            var page = await Browser.NewPageAsync();
            Assert.That(targetTask.IsCompleted, Is.False);
            await page.GoToAsync(TestConstants.EmptyPage);
            var target = await targetTask;
            Assert.That(page, Is.SameAs(await target.PageAsync()));

            await page.CloseAsync();
        }

        [Test, PuppeteerTest("target.spec", "Target Browser.waitForTarget", "should timeout waiting for a non-existent target")]
        public void ShouldTimeoutWaitingForANonExistentTarget()
            => Assert.ThrowsAsync<TimeoutException>(async () => await Browser.WaitForTargetAsync(
                (target) => target.Url == TestConstants.EmptyPage,
                new() { Timeout = 1 }));

        [Test, PuppeteerTest("target.spec", "Target", "should be able to use async waitForTarget")]
        public async Task ShouldBeAbleToUseAsyncWaitForTarget()
        {
            var targetTask = Context.WaitForTargetAsync(
                target => target.Url == TestConstants.CrossProcessHttpPrefix + "/empty.html",
                new() { Timeout = 3000 });
            await Page.EvaluateFunctionAsync(
                "url => window.open(url)",
                TestConstants.CrossProcessHttpPrefix + "/empty.html");
            var target = await targetTask;
            var otherPage = await target.PageAsync();
            Assert.That(otherPage.Url, Is.EqualTo(TestConstants.CrossProcessHttpPrefix + "/empty.html"));
            Assert.That(Page, Is.Not.SameAs(otherPage));
            await otherPage.CloseAsync();
        }
    }
}
