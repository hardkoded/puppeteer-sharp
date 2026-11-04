using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class PageClientTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("page.spec", "Page Page.client", "should return the client instance")]
        public void ShouldReturnTheClientInstance()
        {
            Assert.That(Page.Client, Is.InstanceOf<ICDPSession>());
        }
    }
}
