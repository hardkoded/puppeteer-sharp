using System.Reflection.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InjectedTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InjectedTests : PuppeteerPageBaseTest
    {
        public InjectedTests(): base()
        {
        }

        [PuppeteerTest("injected.spec.ts", "PuppeteerUtil tests", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var world =  (Page.MainFrame as Frame).PuppeteerWorld;
            var result = await world.EvaluateFunctionAsync<bool>(@"
                      PuppeteerUtil => {
                        return typeof PuppeteerUtil === 'object';
                      }",
                      new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)));
            Assert.True(result);
        }

        [PuppeteerTest("injected.spec.ts", "createFunction tests", "should work")]
        [PuppeteerFact]
        public async Task CreateFunctionShouldWork()
        {
            var world = (Page.MainFrame as Frame).PuppeteerWorld;
            var result = await (Page.MainFrame as Frame)
                .PuppeteerWorld.EvaluateFunctionAsync<int>(@"({createFunction}, fnString) => {
                    return createFunction(fnString)(4);
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                "() => 4");
            Assert.Equal(4, result);
        }
    }
}
