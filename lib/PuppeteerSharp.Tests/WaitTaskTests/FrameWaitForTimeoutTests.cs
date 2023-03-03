using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.WaitForTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameWaitForTimeoutTests : PuppeteerPageBaseTest
    {
        public FrameWaitForTimeoutTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("waittask.spec.ts", "Frame.waitForTimeout", "waits for the given timeout before resolving")]
        [PuppeteerFact]
        public async Task WaitsForTheGivenTimeoutBeforeResolving()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var startTime = DateTime.UtcNow;
            await Page.MainFrame.WaitForTimeoutAsync(1000);
            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds > 700);
            Assert.True((DateTime.UtcNow - startTime).TotalMilliseconds < 1300);
        }
    }
}
