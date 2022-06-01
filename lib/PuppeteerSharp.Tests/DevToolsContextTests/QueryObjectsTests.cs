using System.Threading.Tasks;
using CefSharp.Puppeteer;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class QueryObjectsTests : DevToolsContextBaseTest
    {
        public QueryObjectsTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            // Instantiate an object
            await DevToolsContext.EvaluateExpressionAsync("window.set = new Set(['hello', 'world'])");
            var prototypeHandle = await DevToolsContext.EvaluateExpressionHandleAsync("Set.prototype");
            var objectsHandle = await DevToolsContext.QueryObjectsAsync(prototypeHandle);
            var count = await DevToolsContext.EvaluateFunctionAsync<int>("objects => objects.length", objectsHandle);
            Assert.Equal(1, count);
            var values = await DevToolsContext.EvaluateFunctionAsync<string[]>("objects => Array.from(objects[0].values())", objectsHandle);
            Assert.Equal(new[] { "hello", "world" }, values);
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should work for non-blank page")]
        [PuppeteerFact]
        public async Task ShouldWorkForNonBlankPage()
        {
            // Instantiate an object
            await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            await DevToolsContext.EvaluateFunctionAsync("() => window.set = new Set(['hello', 'world'])");
            var prototypeHandle = await DevToolsContext.EvaluateFunctionHandleAsync("() => Set.prototype");
            var objectsHandle = await DevToolsContext.QueryObjectsAsync(prototypeHandle);
            var count = await DevToolsContext.EvaluateFunctionAsync<int>("objects => objects.length", objectsHandle);
            Assert.Equal(1, count);
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should fail for disposed handles")]
        [PuppeteerFact]
        public async Task ShouldFailForDisposedHandles()
        {
            var prototypeHandle = await DevToolsContext.EvaluateExpressionHandleAsync("HTMLBodyElement.prototype");
            await prototypeHandle.DisposeAsync();
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => DevToolsContext.QueryObjectsAsync(prototypeHandle));
            Assert.Equal("Prototype JSHandle is disposed!", exception.Message);
        }

        [PuppeteerTest("page.spec.ts", "ExecutionContext.queryObjects", "should fail primitive values as prototypes")]
        [PuppeteerFact]
        public async Task ShouldFailPrimitiveValuesAsPrototypes()
        {
            var prototypeHandle = await DevToolsContext.EvaluateExpressionHandleAsync("42");
            var exception = await Assert.ThrowsAsync<PuppeteerException>(()
                => DevToolsContext.QueryObjectsAsync(prototypeHandle));
            Assert.Equal("Prototype JSHandle must not be referencing primitive value", exception.Message);
        }
    }
}
