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

        [Test, PuppeteerTimeout, PuppeteerTest("touchscreen.spec", "Touchscreen", "should tap the button")]
        public async Task ShouldTapTheButton()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.AreEqual("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("touchscreen.spec", "Touchscreen", "should report touches")]
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
