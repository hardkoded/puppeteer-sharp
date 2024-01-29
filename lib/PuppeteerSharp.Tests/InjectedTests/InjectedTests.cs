using System.Reflection.Metadata;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.InjectedTests
{
    public class InjectedTests : PuppeteerPageBaseTest
    {
        public InjectedTests() : base()
        {
        }

        [PuppeteerTest("injected.spec.ts", "PuppeteerUtil tests", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await world.EvaluateFunctionAsync<bool>(@"
                      PuppeteerUtil => {
                        return typeof PuppeteerUtil === 'object';
                      }",
                      new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)));
            Assert.True(result);
        }

        [PuppeteerTest("injected.spec.ts", "createFunction tests", "should work")]
        [PuppeteerTimeout]
        public async Task CreateFunctionShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await (Page.MainFrame as Frame)
                .IsolatedRealm.EvaluateFunctionAsync<int>(@"({createFunction}, fnString) => {
                    return createFunction(fnString)(4);
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                "() => 4");
            Assert.AreEqual(4, result);
        }
    }
}
