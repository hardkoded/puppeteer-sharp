using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
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
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.NotNull(windowHandle);
        }
    }
}
