using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class EvaluateHandleTests : PuppeteerPageBaseTest
    {
        public EvaluateHandleTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            var windowHandle = await Page.MainFrame.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}