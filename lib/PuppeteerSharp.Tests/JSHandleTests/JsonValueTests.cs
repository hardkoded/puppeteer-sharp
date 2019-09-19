using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class JsonValueTests : PuppeteerPageBaseTest
    {
        public JsonValueTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("({ foo: 'bar'})");
            var json = await aHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{ foo: 'bar' }"), json);
        }

        [Fact]
        public async Task ShouldNotWorkWithDates()
        {
            var dateHandle = await Page.EvaluateExpressionHandleAsync("new Date('2017-09-26T00:00:00.000Z')");
            var json = await dateHandle.JsonValueAsync();
            Assert.Equal(JObject.Parse("{}"), json);
        }

        [Fact]
        public async Task ShouldThrowForCircularObjects()
        {
            var windowHandle = await Page.EvaluateExpressionHandleAsync("window");
            var exception = await Assert.ThrowsAsync<MessageException>(()
                => windowHandle.JsonValueAsync());
            Assert.Contains("Object reference chain is too long", exception.Message);
        }
    }
}