using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class TitleTests : PuppeteerPageBaseTest
    {
        public TitleTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.title", "should return the page title")]
        [PuppeteerFact]
        public async Task ShouldReturnThePageTitle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
            Assert.Equal("Woof-Woof", await Page.GetTitleAsync());
        }
    }
}
