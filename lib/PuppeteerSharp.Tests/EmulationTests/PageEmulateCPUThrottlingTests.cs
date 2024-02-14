using System.Threading.Tasks;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateCPUThrottlingTests : PuppeteerPageBaseTest
    {
        public PageEmulateCPUThrottlingTests() : base()
        {
        }

        [PuppeteerTest("emulation.spec.ts", "Page.emulateCPUThrottling", "should change the CPU throttling rate successfully")]
        public async Task ShouldChangeTheCPUThrottlingRateSuccessfully()
        {
            await Page.EmulateCPUThrottlingAsync(100);
            await Page.EmulateCPUThrottlingAsync();
        }
    }
}
