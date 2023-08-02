using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class GetPropertyTests : PuppeteerPageBaseTest
    {
        public GetPropertyTests(): base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.getProperty", "should work")]
        [PuppeteerFact]
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