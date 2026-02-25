using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class ToElementTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.toElement", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div class=\"foo\">Foo1</div>");
            var element = await Page.QuerySelectorAsync(".foo");
            var div = await element.ToElementAsync("div");
            Assert.That(div, Is.Not.Null);
        }
    }
}
