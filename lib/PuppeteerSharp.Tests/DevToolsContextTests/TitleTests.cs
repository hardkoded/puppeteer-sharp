using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TitleTests : DevToolsContextBaseTest
    {
        public TitleTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.title", "should return the page title")]
        [PuppeteerFact]
        public async Task ShouldReturnThePageTitle()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/title.html");
            Assert.Equal("Woof-Woof", await DevToolsContext.GetTitleAsync());
        }
    }
}
