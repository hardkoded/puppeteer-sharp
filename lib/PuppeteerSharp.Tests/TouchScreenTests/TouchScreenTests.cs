using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TouchScreenTests
{
    public class TouchScreenTests : PuppeteerPageBaseTest
    {
        private readonly DeviceDescriptor _iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];

        public TouchScreenTests(): base()
        {
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should tap the button")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldTapTheButton()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should report touches")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldReportTouches()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/touches.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.TapAsync();
            Assert.Equal(new string[] {
                "Touchstart: 0",
                "Touchend: 0"
            }, await Page.EvaluateExpressionAsync<string[]>("getResult()"));
        }
    }
}
