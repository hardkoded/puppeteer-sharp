using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class TitleTests : PuppeteerPageBaseTest
    {
        public TitleTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.title", "should return the page title")]
        public async Task ShouldReturnThePageTitle()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/title.html");
            Assert.AreEqual("Woof-Woof", await Page.GetTitleAsync());
        }
    }
}
