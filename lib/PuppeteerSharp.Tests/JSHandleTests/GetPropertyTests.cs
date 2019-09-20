using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class GetPropertyTests : PuppeteerPageBaseTest
    {
        public GetPropertyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");
            var twoHandle = await aHandle.GetPropertyAsync("two");
            Assert.Equal(2, await twoHandle.JsonValueAsync<int>());
        }
    }
}