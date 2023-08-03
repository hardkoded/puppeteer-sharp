using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.WaitForTests
{
    public class PageWaitForTimeoutTests : PuppeteerPageBaseTest
    {
        public PageWaitForTimeoutTests(): base()
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Page.waitForTimeout", "waits for the given timeout before resolving")]
        [PuppeteerTimeout]
        public async Task WaitsForTheGivenTimeoutBeforeResolving()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var startTime = DateTime.UtcNow;
            await Page.WaitForTimeoutAsync(1000);
            var endTime = DateTime.UtcNow;
            Assert.True((endTime - startTime).TotalMilliseconds > 700);
            Assert.True((endTime - startTime).TotalMilliseconds < 1300);
        }
    }
}
