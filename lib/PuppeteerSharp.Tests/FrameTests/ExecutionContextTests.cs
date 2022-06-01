using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ExecutionContextTests : DevToolsContextBaseTest
    {
        public ExecutionContextTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.executionContext", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, DevToolsContext.Frames.Length);

            var context1 = await DevToolsContext.MainFrame.GetExecutionContextAsync();
            var context2 = await DevToolsContext.FirstChildFrame().GetExecutionContextAsync();
            Assert.NotNull(context1);
            Assert.NotNull(context2);
            Assert.NotEqual(context1, context2);
            Assert.Equal(DevToolsContext.MainFrame, context1.Frame);
            Assert.Equal(DevToolsContext.FirstChildFrame(), context2.Frame);

            await Task.WhenAll(
                context1.EvaluateExpressionAsync("window.a = 1"),
                context2.EvaluateExpressionAsync("window.a = 2")
            );

            var a1 = context1.EvaluateExpressionAsync<int>("window.a");
            var a2 = context2.EvaluateExpressionAsync<int>("window.a");

            await Task.WhenAll(a1, a2);

            Assert.Equal(1, a1.Result);
            Assert.Equal(2, a2.Result);
        }
    }
}
