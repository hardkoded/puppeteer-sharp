using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        public EvaluateHandleTests() : base()
        {
        }

        [Test, PuppeteerTest("frame.spec", "Frame.evaluateHandle", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await Page.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}
