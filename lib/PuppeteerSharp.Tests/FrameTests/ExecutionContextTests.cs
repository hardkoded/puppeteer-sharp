using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class ExecutionContextTests : PuppeteerPageBaseTest
    {
        public ExecutionContextTests(): base()
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.executionContext", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.AreEqual(2, Page.Frames.Length);

            var context1 = await Page.MainFrame.GetExecutionContextAsync();
            var context2 = await Page.FirstChildFrame().GetExecutionContextAsync();
            Assert.NotNull(context1);
            Assert.NotNull(context2);
            Assert.NotEqual(context1, context2);
            Assert.AreEqual(Page.MainFrame, context1.Frame);
            Assert.AreEqual(Page.FirstChildFrame(), context2.Frame);

            await Task.WhenAll(
                context1.EvaluateExpressionAsync("window.a = 1"),
                context2.EvaluateExpressionAsync("window.a = 2")
            );

            var a1 = context1.EvaluateExpressionAsync<int>("window.a");
            var a2 = context2.EvaluateExpressionAsync<int>("window.a");

            await Task.WhenAll(a1, a2);

            Assert.AreEqual(1, a1.Result);
            Assert.AreEqual(2, a2.Result);
        }
    }
}
