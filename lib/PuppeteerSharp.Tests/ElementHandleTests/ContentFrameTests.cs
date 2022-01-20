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
        public ContentFrameTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.contentFrame", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            var elementHandle = await DevToolsContext.QuerySelectorAsync("#frame1");
            var frame = await elementHandle.ContentFrameAsync();
            Assert.Equal(DevToolsContext.FirstChildFrame(), frame);
        }
    }
}