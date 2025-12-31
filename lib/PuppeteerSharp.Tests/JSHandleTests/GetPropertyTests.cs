using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class GetPropertyTests : PuppeteerPageBaseTest
    {
        public GetPropertyTests() : base()
        {
        }

        [Test, PuppeteerTest("jshandle.spec", "JSHandle JSHandle.getProperty", "should work")]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync(@"({
              one: 1,
              two: 2,
              three: 3
            })");
            var twoHandle = await aHandle.GetPropertyAsync("two");
            Assert.That(await twoHandle.JsonValueAsync<int>(), Is.EqualTo(2));
        }
    }
}
