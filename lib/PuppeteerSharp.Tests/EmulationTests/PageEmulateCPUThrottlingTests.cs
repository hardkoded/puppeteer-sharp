using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEmulateCPUThrottlingTests : DevToolsContextBaseTest
    {
        public PageEmulateCPUThrottlingTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateCPUThrottling", "should change the CPU throttling rate successfully")]
        public async Task ShouldChangeTheCPUThrottlingRateSuccessfully()
        {
            await DevToolsContext.EmulateCPUThrottlingAsync(100);
            await DevToolsContext.EmulateCPUThrottlingAsync();
        }
    }
}
