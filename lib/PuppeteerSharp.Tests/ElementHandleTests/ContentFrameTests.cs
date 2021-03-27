using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ContentFrameTests : PuppeteerPageBaseTest
    {
        public ContentFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.contentFrame", "should work")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var elementHandle = await Page.QuerySelectorAsync("#frame1");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.Equal(Page.FirstChildFrame(), frame);
        }
    }
}