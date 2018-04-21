using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Target
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Tests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task BrowserTargetsShouldReturnAllOfTheTargets()
        {
            // The pages will be the testing page and the original newtab page
            var targets = Browser.Targets();
            Assert.Contains(targets, target => target.Type == "page"
                && target.Url == TestConstants.AboutBlank);
            Assert.Contains(targets, target => target.Type == "other"
                && target.Url == string.Empty);
        }

        [Fact]
        public async Task BrowserPagesShouldReturnAllOfThePages()
        {
            // The pages will be the testing page and the original newtab page
            var allPages = (await Browser.Pages()).ToArray();
            Assert.Equal(2, allPages.Length);
            Assert.Contains(Page, allPages);
            Assert.NotSame(allPages[0], allPages[1]);
        }
    }
}