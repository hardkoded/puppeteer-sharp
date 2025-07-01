using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EvaluationTests
{
    public class FrameEvaluateTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Frame.evaluate", "should have different execution contexts")]
        public async Task ShouldHaveDifferentExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.That(Page.Frames, Has.Length.EqualTo(2));

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            await frame1.EvaluateExpressionAsync("window.FOO = 'foo'");
            await frame2.EvaluateExpressionAsync("window.FOO = 'bar'");

            Assert.That(await frame1.EvaluateExpressionAsync<string>("window.FOO"), Is.EqualTo("foo"));
            Assert.That(await frame2.EvaluateExpressionAsync<string>("window.FOO"), Is.EqualTo("bar"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Frame.evaluate", "should have correct execution contexts")]
        public async Task ShouldHaveCorrectExecutionContexts()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.That(Page.Frames, Has.Length.EqualTo(2));

            var frame1 = Page.MainFrame;
            var frame2 = Page.FirstChildFrame();

            Assert.That(await frame1.EvaluateExpressionAsync<string>("document.body.textContent.trim()"), Is.EqualTo(""));
            Assert.That(await frame2.EvaluateExpressionAsync<string>("document.body.textContent.trim()"), Is.EqualTo("Hi, I'm frame"));
        }

        [Test, PuppeteerTest("evaluation.spec", "Evaluation specs Frame.evaluate", "should execute after cross-site navigation")]
        public async Task ShouldExecuteAfterCrossSiteNavigation()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var mainFrame = Page.MainFrame;
            Assert.That(await mainFrame.EvaluateExpressionAsync<string>("window.location.href"), Does.Contain("localhost"));

            await Page.GoToAsync(TestConstants.CrossProcessHttpPrefix + "/empty.html");
            Assert.That(await mainFrame.EvaluateExpressionAsync<string>("window.location.href"), Does.Contain("127"));
        }
    }
}
