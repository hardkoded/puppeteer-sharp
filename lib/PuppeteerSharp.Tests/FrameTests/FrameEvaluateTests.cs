using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.evaluate", "should throw for detached frames")]
        [PuppeteerFact]
        public async Task ShouldThrowForDetachedFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.FirstChildFrame();
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var exception = await Assert.ThrowsAnyAsync<PuppeteerException>(
                () => frame.EvaluateExpressionAsync("5 * 8"));
            Assert.Contains("Execution Context is not available in detached frame", exception.Message);
        }
    }
}