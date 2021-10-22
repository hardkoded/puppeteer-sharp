using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEmulateCPUThrottlingTests : PuppeteerPageBaseTest
    {
        public PageEmulateCPUThrottlingTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateCPUThrottling", "should change the CPU throttling rate successfully")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldChangeTheCPUThrottlingRateSuccessfully()
        {
            await Page.EmulateCPUThrottlingAsync(100);
            await Page.EmulateCPUThrottlingAsync();
        }
    }
}
