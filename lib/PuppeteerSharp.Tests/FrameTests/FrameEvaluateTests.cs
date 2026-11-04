using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.evaluate", "should throw for detached frames")]
        public async Task ShouldThrowForDetachedFrames()
        {
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            var frame = await Page.FirstChildFrameAsync();
            await FrameUtils.DetachFrameAsync(Page, "frame1");
            var exception = Assert.ThrowsAsync<PuppeteerException>(
                () => frame.EvaluateExpressionAsync("5 * 8"));
            Assert.That(exception.Message, Does.Contain("Execution Context is not available in detached frame"));
        }

        [Test, PuppeteerTest("frame.spec", "Frame specs Frame.evaluate", "allows readonly array to be an argument")]
        public async Task AllowsReadonlyArrayToBeAnArgument()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;

            // This test checks if Frame.evaluate allows an array to be an argument.
            // See https://github.com/puppeteer/puppeteer/issues/6953.
            var readonlyArray = new[] { "a", "b", "c" };
            await mainFrame.EvaluateFunctionAsync("arr => arr", (object)readonlyArray);
        }
    }
}
