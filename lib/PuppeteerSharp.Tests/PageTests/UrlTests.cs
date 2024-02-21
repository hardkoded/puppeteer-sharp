using System;
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

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.url", "should work")]
        public async Task ShouldWork()
        {
            Assert.AreEqual(TestConstants.AboutBlank, Page.Url);
            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.AreEqual(TestConstants.EmptyPage, Page.Url);
        }
    }
}
