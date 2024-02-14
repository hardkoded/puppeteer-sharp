using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NavigationTests
{
    public class PageReloadTests : PuppeteerPageBaseTest
    {
        public PageReloadTests() : base()
        {
        }

        [Test, PuppeteerTest("navigation.spec", "navigation Page.reload", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateFunctionAsync("() => (globalThis._foo = 10)");
            await Page.ReloadAsync();
            Assert.Null(await Page.EvaluateFunctionAsync("() => globalThis._foo"));
        }
    }
}
