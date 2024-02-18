using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("frame.spec", "Frame.evaluate", "should throw for detached frames")]
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
