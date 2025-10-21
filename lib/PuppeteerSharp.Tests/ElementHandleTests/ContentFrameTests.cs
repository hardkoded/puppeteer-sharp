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

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.contentFrame", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var elementHandle = await Page.QuerySelectorAsync("#frame1");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.That(frame, Is.EqualTo(Page.FirstChildFrame()));
        }

        [Test, PuppeteerTest("elementhandle.spec", "PuppeteerSharp", "should work headful")]
        public async Task ShouldWorkHeadful()
        {
            await using var browser = await Puppeteer.LaunchAsync(_headfulOptions);
            await using var page = await browser.NewPageAsync();
            await page.GoToAsync($"{TestConstants.ServerUrl}/frame-example.html");
            var elementHandle = await page.QuerySelectorAsync("iframe");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.That(frame, Is.EqualTo(page.FirstChildFrame()));
        }
    }
}
