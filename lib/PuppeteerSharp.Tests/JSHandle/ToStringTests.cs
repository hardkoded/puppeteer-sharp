using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.JSHandle
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ToStringTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await Page.EvaluateExpressionHandleAsync("2");
            Assert.Equal("JSHandle:2", numberHandle.ToString());
            var stringHandle = await Page.EvaluateExpressionHandleAsync("'a'");
            Assert.Equal("JSHandle:a", stringHandle.ToString());
        }

        [Fact]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.Equal("JSHandle@object", aHandle.ToString());
        }
    }
}