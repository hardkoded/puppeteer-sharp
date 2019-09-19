using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AsElementTests : PuppeteerPageBaseTest
    {
        public AsElementTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.body");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
        }

        [Fact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("2");
            var element = aHandle as ElementHandle;
            Assert.Null(element);
        }

        [Fact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await Page.SetContentAsync("<div>ee!</div>");
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.querySelector('div').firstChild");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
            Assert.True(await Page.EvaluateFunctionAsync<bool>("e => e.nodeType === HTMLElement.TEXT_NODE", element));
        }
    }
}