using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class UrlTests : PuppeteerPageBaseTest
    {
        public UrlTests(): base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.url", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            Assert.Equal(TestConstants.AboutBlank, Page.Url);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(TestConstants.EmptyPage, Page.Url);
        }
    }
}
