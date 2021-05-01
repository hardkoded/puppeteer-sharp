using System.Threading.Tasks;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ToStringTests : PuppeteerPageBaseTest
    {
        public ToStringTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for primitives")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkForPrimitives()
        {
            var numberHandle = await Page.EvaluateExpressionHandleAsync("2");
            Assert.Equal("JSHandle:2", numberHandle.ToString());
            var stringHandle = await Page.EvaluateExpressionHandleAsync("'a'");
            Assert.Equal("JSHandle:a", stringHandle.ToString());
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.toString", "should work for complicated objects")]
        [Fact(Timeout = TestConstants.DefaultTestTimeout)]
        public async Task ShouldWorkForComplicatedObjects()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("window");
            Assert.Equal("JSHandle@object", aHandle.ToString());
        }
    }
}