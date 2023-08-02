using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PuppeteerSharp.Tests.TargetTests
{
    public class BrowserWaitForTargetTests : PuppeteerPageBaseTest
    {
        public BrowserWaitForTargetTests(): base()
        {
        }

        [PuppeteerTest("target.spec.ts", "Browser.waitForTarget", "should wait for a target")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWaitForATarget()
        {
            var targetTask = Browser.WaitForTargetAsync((target) => target.Url == TestConstants.EmptyPage);
            var page = await Browser.NewPageAsync();
            Assert.False(targetTask.IsCompleted);
            await page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(targetTask.IsCompleted);
            Assert.Same(await targetTask.Result.PageAsync(), page);
            
            await page.CloseAsync();
        }

        [PuppeteerTest("target.spec.ts", "Browser.waitForTarget", "should timeout waiting for a non-existent target")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public Task ShouldTimeoutWaitingForANonExistentTarget()
            => Assert.ThrowsAnyAsync<TimeoutException>(async () => await Browser.WaitForTargetAsync(
                (target) => target.Url == TestConstants.EmptyPage,
                new() { Timeout = 1}));
    }
}
