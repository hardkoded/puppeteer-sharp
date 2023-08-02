using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        public FrameEvaluateTests(): base()
        {
        }

        [PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should have different execution contexts")]
        [PuppeteerTimeout]
        public async Task ShouldHaveDifferentExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, Page.Frames.Count());

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            await frame1.EvaluateExpressionAsync("window.FOO = 'foo'");
            await frame2.EvaluateExpressionAsync("window.FOO = 'bar'");

            Assert.Equal("foo", await frame1.EvaluateExpressionAsync<string>("window.FOO"));
            Assert.Equal("bar", await frame2.EvaluateExpressionAsync<string>("window.FOO"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should have correct execution contexts")]
        [PuppeteerTimeout]
        public async Task ShouldHaveCorrectExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.Equal(2, Page.Frames.Count());

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            Assert.Equal("", await frame1.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            Assert.Equal("Hi, I'm frame", await frame2.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should execute after cross-site navigation")]
        [PuppeteerTimeout]
        public async Task ShouldExecuteAfterCrossSiteNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            Assert.Contains("localhost", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));

            await Page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/empty.html");
            Assert.Contains("127", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));
        }
    }
}
