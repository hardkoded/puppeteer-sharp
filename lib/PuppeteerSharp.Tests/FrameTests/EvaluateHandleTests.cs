using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.FrameTests
{
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        public EvaluateHandleTests(): base()
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.evaluateHandle", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await Page.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}