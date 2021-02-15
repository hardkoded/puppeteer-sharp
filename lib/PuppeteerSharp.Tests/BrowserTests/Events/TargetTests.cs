using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.BrowserTests.Events
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TargetTests : PuppeteerBrowserBaseTest
    {
        public TargetTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            var events = new List<string>();
            Browser.TargetCreated += (_, _) => events.Add("CREATED");
            Browser.TargetChanged += (_, _) => events.Add("CHANGED");
            Browser.TargetDestroyed += (_, _) => events.Add("DESTROYED");
            var page = await Browser.NewPageAsync();
            await page.GoToAsync(TestConstants.EmptyPage);
            await page.CloseAsync();
            Assert.Equal(new[] { "CREATED", "CHANGED", "DESTROYED" }, events);
        }
    }
}
