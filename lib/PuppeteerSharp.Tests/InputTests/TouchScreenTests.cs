using System.Threading.Tasks;
using PuppeteerSharp.Mobile;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InputTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class TouchScreenTests : PuppeteerPageBaseTest
    {
        private readonly DeviceDescriptor _iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];

        public TouchScreenTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldTapTheButton()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.Equal("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Fact]
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
