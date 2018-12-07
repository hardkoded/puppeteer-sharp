using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ContentFrameTests : PuppeteerPageBaseTest
    {
        public ContentFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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