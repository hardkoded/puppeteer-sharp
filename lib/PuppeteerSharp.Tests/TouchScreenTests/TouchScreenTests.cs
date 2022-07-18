using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using CefSharp.DevTools.Dom.Mobile;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.TouchScreenTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TouchScreenTests : DevToolsContextBaseTest
    {
        private readonly DeviceDescriptor _iPhone = Emulation.Devices[DeviceDescriptorName.IPhone6];

        public TouchScreenTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should tap the button")]
        [PuppeteerFact]
        public async Task ShouldTapTheButton()
        {
            await DevToolsContext.EmulateAsync(_iPhone);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await DevToolsContext.TapAsync("button");
            Assert.Equal("Clicked", await DevToolsContext.EvaluateExpressionAsync<string>("result"));
        }

        [PuppeteerTest("touchscreen.spec.ts", "Touchscreen", "should report touches")]
        [PuppeteerFact]
        public async Task ShouldReportTouches()
        {
            await DevToolsContext.EmulateAsync(_iPhone);
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/touches.html");
            var button = await DevToolsContext.QuerySelectorAsync("button");
            await button.TapAsync();
            Assert.Equal(new string[] {
                "Touchstart: 0",
                "Touchend: 0"
            }, await DevToolsContext.EvaluateExpressionAsync<string[]>("getResult()"));
        }
    }
}
