using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateCPUThrottlingTests : PuppeteerPageBaseTest
    {
        public PageEmulateCPUThrottlingTests(): base()
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
