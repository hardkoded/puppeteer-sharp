using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TargetTests
{
    public class BrowserWaitForTargetTests : PuppeteerPageBaseTest
    {
        public BrowserWaitForTargetTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("target.spec", "Target Browser.waitForTarget", "should wait for a target")]
        public async Task ShouldWaitForATarget()
        {
            var targetTask = Browser.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await Browser.NewPageAsync();
            Assert.False(targetTask.IsCompleted);
            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(targetTask.IsCompleted);
            Assert.AreSame(await targetTask.Result.PageAsync(), page);

            await page.CloseAsync();
        }

        [Test, Retry(2), PuppeteerTest("target.spec", "Target Browser.waitForTarget", "should timeout waiting for a non-existent target")]
        public void ShouldTimeoutWaitingForANonExistentTarget()
            => Assert.ThrowsAsync<TimeoutException>(async () => await Browser.WaitForTargetAsync(
                (target) => target.Url == TestConstants.EmptyPage,
                new() { Timeout = 1 }));
    }
}
