using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.InjectedTests
{
    public class InjectedTests : PuppeteerPageBaseTest
    {
        public InjectedTests() : base()
        {
        }

        [Test, PuppeteerTest("injected.spec", "PuppeteerUtil tests", "should work")]
        public async Task ShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await world.EvaluateFunctionAsync<bool>(@"
                      PuppeteerUtil => {
                        return typeof PuppeteerUtil === 'object';
                      }",
                      new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)));
            Assert.That(result, Is.True);
        }

        [Test, PuppeteerTest("injected.spec", "createFunction tests", "should work")]
        public async Task CreateFunctionShouldWork()
        {
            var world = (Page.MainFrame as Frame).IsolatedRealm;
            var result = await (Page.MainFrame as Frame)
                .IsolatedRealm.EvaluateFunctionAsync<int>(@"({createFunction}, fnString) => {
                    return createFunction(fnString)(4);
                }",
                new LazyArg(async context => await context.GetPuppeteerUtilAsync().ConfigureAwait(false)),
                "() => 4");
            Assert.That(result, Is.EqualTo(4));
        }
    }
}
