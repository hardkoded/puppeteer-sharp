using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        public EvaluateHandleTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("frame.spec", "Frame.evaluateHandle", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await Page.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}
