using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests() : base()
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.evaluate", "should throw for detached frames")]
        [PuppeteerTimeout]
        public async Task ShouldThrowForDetachedFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = Page.FirstChildFrame();
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var exception = Assert.ThrowsAsync<PuppeteerException>(
                () => frame.EvaluateExpressionAsync("5 * 8"));
            StringAssert.Contains("Execution Context is not available in detached frame", exception.Message);
        }
    }
}
