using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class TitleTests : PuppeteerPageBaseTest
    {
        public TitleTests() : base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.title", "should return the page title")]
        [PuppeteerTimeout]
        public async Task ShouldReturnThePageTitle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
            Assert.AreEqual("Woof-Woof", await Page.GetTitleAsync());
        }
    }
}
