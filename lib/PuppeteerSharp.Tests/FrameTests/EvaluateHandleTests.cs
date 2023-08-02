using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        public EvaluateHandleTests(): base()
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.evaluateHandle", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await Page.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}