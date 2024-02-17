using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class PageEmulateCPUThrottlingTests : PuppeteerPageBaseTest
    {
        public PageEmulateCPUThrottlingTests() : base()
        {
        }

        [Test, PuppeteerTimeout, PuppeteerTest("emulation.spec", "Emulation Page.emulateCPUThrottling", "should change the CPU throttling rate successfully")]
        public async Task ShouldChangeTheCPUThrottlingRateSuccessfully()
        {
            await Page.EmulateCPUThrottlingAsync(100);
            await Page.EmulateCPUThrottlingAsync();
        }
    }
}
