using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.TouchScreenTests
{
    public class TouchScreenTests : PuppeteerPageBaseTest
    {
        private readonly DeviceDescriptor _iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];

        public TouchScreenTests() : base()
        {
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should tap the button")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldTapTheButton()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.AreEqual("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should report touches")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReportTouches()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/touches.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.TapAsync();
            Assert.AreEqual(new string[] {
                "Touchstart: 0",
                "Touchend: 0"
            }, await Page.EvaluateExpressionAsync<string[]>("getResult()"));
        }
    }
}
