using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should have different execution contexts")]
        [PuppeteerTimeout]
        public async Task ShouldHaveDifferentExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.AreEqual(2, Page.Frames.Count());

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            await frame1.EvaluateExpressionAsync("window.FOO = 'foo'");
            await frame2.EvaluateExpressionAsync("window.FOO = 'bar'");

            Assert.AreEqual("foo", await frame1.EvaluateExpressionAsync<string>("window.FOO"));
            Assert.AreEqual("bar", await frame2.EvaluateExpressionAsync<string>("window.FOO"));
        }

        [Test, PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should have correct execution contexts")]
        [PuppeteerTimeout]
        public async Task ShouldHaveCorrectExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.AreEqual(2, Page.Frames.Count());

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            Assert.AreEqual("", await frame1.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
            Assert.AreEqual("Hi, I'm frame", await frame2.EvaluateExpressionAsync<string>("document.body.textContent.trim()"));
        }

        [Test, PuppeteerTest("evaluation.spec.ts", "Frame.evaluate", "should execute after cross-site navigation")]
        [PuppeteerTimeout]
        public async Task ShouldExecuteAfterCrossSiteNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            StringAssert.Contains("localhost", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));

            await Page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/empty.html");
            StringAssert.Contains("127", await mainFrame.EvaluateExpressionAsync<string>("window.location.href"));
        }
    }
}
