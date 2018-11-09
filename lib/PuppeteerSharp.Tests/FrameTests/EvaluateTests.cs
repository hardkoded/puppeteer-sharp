using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateTests : PuppeteerPageBaseTest
    {
        public EvaluateTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
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

        [Fact]
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
