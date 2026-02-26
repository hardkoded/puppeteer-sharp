using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QueryHandlerTests.XPathSelectorTests
{
    public class XPathSelectorInElementHandlesTests : PuppeteerPageBaseTest
    {
        public XPathSelectorInElementHandlesTests() : base()
        {
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in ElementHandles", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\">a<span></span></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.That(await elementHandle.QuerySelectorAsync("xpath/span"), Is.Not.Null);
            Assert.That(await elementHandle.QuerySelectorAllAsync("xpath/span"), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests XPath selectors in ElementHandles", "should return null for non-existing element")]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\">a</div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.That(await elementHandle.QuerySelectorAsync("xpath/span"), Is.Null);
            Assert.That(await elementHandle.QuerySelectorAllAsync("xpath/span"), Is.Empty);
        }
    }
}
