using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class TabInfoTests : PuppeteerBrowserContextBaseTest
    {
        [Test, PuppeteerTest("page.test", "Page Page.tabInfo", "should return the tab index and active state")]
        public async Task ShouldReturnTheTabIndexAndActiveState()
        {
            var page1 = await Context.NewPageAsync();
            var page2 = await Context.NewPageAsync();

            await page1.BringToFrontAsync();
            var tabInfo1 = await page1.TabInfoAsync();
            var tabInfo2 = await page2.TabInfoAsync();
            Assert.That(tabInfo1.Active, Is.True);
            Assert.That(tabInfo1.Pinned, Is.False);
            Assert.That(tabInfo2.Active, Is.False);
            Assert.That(tabInfo2.Pinned, Is.False);
            Assert.That(tabInfo2.Index, Is.EqualTo(tabInfo1.Index + 1));
            Assert.That(tabInfo1.GroupId, Is.Null);
            Assert.That(tabInfo2.GroupId, Is.Null);

            await page2.BringToFrontAsync();
            Assert.That((await page1.TabInfoAsync()).Active, Is.False);
            Assert.That((await page2.TabInfoAsync()).Active, Is.True);

            await page1.CloseAsync();
            await page2.CloseAsync();
        }
    }
}
