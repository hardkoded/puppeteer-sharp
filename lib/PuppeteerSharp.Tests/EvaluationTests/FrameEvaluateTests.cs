using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should have different execution contexts")]
        [PuppeteerFact]
        public async Task ShouldHaveDifferentExecutionContexts()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(DevToolsContext, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, DevToolsContext.Frames.Count());

            var frame1 = DevToolsContext.MainFrame;
            var frame2 = DevToolsContext.FirstChildFrame();

            await frame1.EvaluateExpressionAsync("window.FOO = 'foo'");
            await frame2.EvaluateExpressionAsync("window.FOO = 'bar'");

            Assert.Equal("foo", await frame1.EvaluateExpressionAsync<string>("window.FOO"));
            Assert.Equal("bar", await frame2.EvaluateExpressionAsync<string>("window.FOO"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should execute after cross-site navigation")]
        [PuppeteerFact]
        public async Task ShouldExecuteAfterCrossSiteNavigation()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = DevToolsContext.MainFrame;
            Assert.Contains("localhost", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));

            await DevToolsContext.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/empty.html");
            Assert.Contains("127", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));
        }
    }
}
