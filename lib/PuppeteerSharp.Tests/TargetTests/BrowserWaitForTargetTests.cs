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
            var targetTask = Browser.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await Browser.NewPageAsync();
            Assert.That(targetTask.IsCompleted, Is.False);
            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(targetTask.IsCompleted, Is.True);
            Assert.That(page, Is.SameAs(await targetTask.Result.PageAsync()));

            await page.CloseAsync();
        }

        [Test, PuppeteerTest("target.spec", "Target Browser.waitForTarget", "should timeout waiting for a non-existent target")]
        public void ShouldTimeoutWaitingForANonExistentTarget()
            => Assert.ThrowsAsync<TimeoutException>(async () => await Browser.WaitForTargetAsync(
                (target) => target.Url == TestConstants.EmptyPage,
                new() { Timeout = 1 }));
    }
}
