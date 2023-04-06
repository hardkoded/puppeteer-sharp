using System.Reflection.Metadata;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.InjectedTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InjectedTests : PuppeteerPageBaseTest
    {
        public InjectedTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("injected.spec.ts", "InjectedUtil tests", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            var result = await (Page.MainFrame as Frame)
                .SecondaryWorld.EvaluateFunctionAsync<bool>(@"() => {
                      return typeof InjectedUtil === 'object';
                  }");
            Assert.True(result);
        }
    }
}
