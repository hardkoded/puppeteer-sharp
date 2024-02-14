using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QueryHandlerTests.TextSelectorTests
{
    public class TextSelectorInElementHandlesTests : PuppeteerPageBaseTest
    {
        public TextSelectorInElementHandlesTests() : base()
        {
        }

        [Test, PuppeteerTest("queryhandler.spec.ts", "in ElementHandles", "should query existing element")]
        [PuppeteerTimeout]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\"><span>a</span></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.NotNull(await elementHandle.QuerySelectorAsync("text/a"));
            Assert.That(await elementHandle.QuerySelectorAllAsync("text/a"), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTest("queryhandler.spec.ts", "in Page", "should return null for non-existing element")]
        [PuppeteerTimeout]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\"></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.Null(await elementHandle.QuerySelectorAsync("text/a"));
            Assert.IsEmpty(await elementHandle.QuerySelectorAllAsync("text/a"));
        }
    }
}
