using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.PageTests
{
    public class UrlTests : PuppeteerPageBaseTest
    {
        public UrlTests() : base()
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.url", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            Assert.AreEqual(TestConstants.AboutBlank, Page.Url);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(TestConstants.EmptyPage, Page.Url);
        }
    }
}
