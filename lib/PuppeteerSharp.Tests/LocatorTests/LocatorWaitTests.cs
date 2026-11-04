using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.LocatorTests
{
    public class LocatorWaitTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("locator.spec", "Locator.prototype.wait", "should work")]
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

            // This shouldn't throw.
            await Page.Locator("div").WaitAsync();
        }
    }
}
