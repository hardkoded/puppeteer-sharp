using Microsoft.AspNetCore.Http;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TargetTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class BrowserWaitForTargetTests : PuppeteerPageBaseTest
    {
        public BrowserWaitForTargetTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("target.spec.ts", "Browser.waitForTarget", "should wait for a target")]
        [SkipBrowserFact(skipFirefox: true)]
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
        [SkipBrowserFact(skipFirefox: true)]
        public Task ShouldTimeoutWaitingForANonExistentTarget()
            => Assert.ThrowsAnyAsync<TimeoutException>(async () => await Browser.WaitForTargetAsync(
                (target) => target.Url == TestConstants.EmptyPage,
                new() { Timeout = 1}));
    }
}
