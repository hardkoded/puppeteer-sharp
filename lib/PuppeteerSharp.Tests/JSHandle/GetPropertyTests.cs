using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.JSHandle
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GetPropertyTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");
            var twoHandle = await aHandle.GetPropertyAsync("two");
            Assert.Equal(2, await twoHandle.JsonValue<int>());
        }
    }
}