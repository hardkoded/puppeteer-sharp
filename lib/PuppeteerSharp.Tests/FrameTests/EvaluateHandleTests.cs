using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EvaluateHandleTests : DevToolsContextBaseTest
    {
        public EvaluateHandleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("frame.spec.ts", "Frame.evaluateHandle", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await DevToolsContext.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}