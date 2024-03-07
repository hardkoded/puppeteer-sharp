using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ContentFrameTests : PuppeteerPageBaseTest
    {
        private readonly LaunchOptions _headfulOptions;

        public ContentFrameTests() : base()
        {
            _headfulOptions = TestConstants.DefaultBrowserOptions();
            _headfulOptions.Headless = false;
        }

        [Test, Retry(2), PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.contentFrame", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var elementHandle = await Page.QuerySelectorAsync("#frame1");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.AreEqual(Page.FirstChildFrame(), frame);
        }

        public async Task ShouldWorkHeadful()
        {
            await using var Browser = await Puppeteer.LaunchAsync(_headfulOptions);
            await using var Page = await Browser.NewPageAsync();
            await Page.GoToAsync($"{TestConstants.ServerUrl}/frame-example.html");
            var elementHandle = await Page.QuerySelectorAsync("iframe");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.AreEqual(Page.FirstChildFrame(), frame);
        }
    }
}
