using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class UrlTests : PuppeteerPageBaseTest
    {
        public UrlTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.url", "should work")]
        public async Task ShouldWork()
        {
            Assert.That(Page.Url, Is.EqualTo(TestConstants.AboutBlank));
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(Page.Url, Is.EqualTo(TestConstants.EmptyPage));
        }
    }
}
