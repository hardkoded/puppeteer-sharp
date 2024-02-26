using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Mobile;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.TouchScreenTests
{
    public class TouchScreenTests : PuppeteerPageBaseTest
    {
        private readonly DeviceDescriptor _iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];

        public TouchScreenTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("touchscreen.spec", "Touchscreen", "should tap the button")]
        public async Task ShouldTapTheButton()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            await Page.TapAsync("button");
            Assert.AreEqual("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }

        [Test, Retry(2), PuppeteerTest("touchscreen.spec", "Touchscreen", "should report touches")]
        public async Task ShouldReportTouches()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/touches.html");
            var button = await Page.QuerySelectorAsync("button");
            await button.TapAsync();
            Assert.AreEqual(new string[] { "Touchstart: 0", "Touchend: 0" },
                await Page.EvaluateExpressionAsync<string[]>("getResult()"));
        }

        [Test, Retry(2), PuppeteerTest("touchscreen.spec", "Touchscreen", "should report touchMove")]
        public async Task ShouldReportTouchMove()
        {
            await Page.EmulateAsync(_iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/touch-move.html");
            var touch = await Page.QuerySelectorAsync("#touch");
            var touchObj = await touch.BoundingBoxAsync();
            await Page.Touchscreen.TouchStartAsync(touchObj.X, touchObj.Y);
            var movePosx = 100;
            var movePosy = 100;
            await Page.Touchscreen.TouchMoveAsync(movePosx, movePosy);
            await Page.Touchscreen.TouchEndAsync();
            Assert.AreEqual(movePosx, await Page.EvaluateExpressionAsync<int>("touchX"));
            Assert.AreEqual(movePosy, await Page.EvaluateExpressionAsync<int>("touchY"));
        }
    }
}
