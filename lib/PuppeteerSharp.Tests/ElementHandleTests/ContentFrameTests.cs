using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ContentFrameTests : PuppeteerPageBaseTest
    {
        private readonly LaunchOptions _headfulOptions;

        public ContentFrameTests(ITestOutputHelper output) : base(output)
        {
            _headfulOptions = TestConstants.DefaultBrowserOptions();
            _headfulOptions.Headless =  false;
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.contentFrame", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var elementHandle = await Page.QuerySelectorAsync("#frame1");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.Equal(Page.FirstChildFrame(), frame);
        }

        [PuppeteerFact]
        public async Task ShouldWorkHeadful()
        {
            await using var Browser = await Puppeteer.LaunchAsync(_headfulOptions);
            await using var Page = await Browser.NewPageAsync();
            await Page.GoToAsync($"{TestConstants.ServerUrl}/frame-example.html");
            var elementHandle = await Page.QuerySelectorAsync("iframe");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.Equal(Page.FirstChildFrame(), frame);
        }
    }
}