using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class DisposeTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.dispose", "should dispose cached isolated handler")]
        public async Task ShouldDisposeCachedIsolatedHandler()
        {
            await Page.SetContentAsync("<button>Click me!</button>");

            var button = await Page.WaitForSelectorAsync("button");

            // Cache the handle on the isolated world
            await button.ClickAsync();

            await button.DisposeAsync();
            Assert.That(button.Disposed, Is.True);
        }
    }
}
