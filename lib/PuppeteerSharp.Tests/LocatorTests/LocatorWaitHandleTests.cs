using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorWaitHandleTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator.prototype.waitHandle", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync(@"
                <script>
                    setTimeout(() => {
                        const element = document.createElement('div');
                        element.innerText = 'test2';
                        document.body.append(element);
                    }, 50);
                </script>");

            var handle = await Page.Locator("div").WaitHandleAsync();
            Assert.That(handle, Is.Not.Null);
            await handle.DisposeAsync();
        }
    }
}
