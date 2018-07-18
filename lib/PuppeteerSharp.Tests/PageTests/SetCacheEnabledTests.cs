using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetCacheEnabledTests : PuppeteerPageBaseTest
    {
        public SetCacheEnabledTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEnableOrDisableTheCacheBasedOnTheStatePassed()
        {
            var responses = new Dictionary<string, Response>();
            Page.Response += (sender, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;

            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html",
                waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            await Page.ReloadAsync(waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            Assert.True(responses["one-style.css"].FromCache);

            await Page.SetCacheEnabledAsync(false);
            await Page.ReloadAsync(waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            Assert.False(responses["one-style.css"].FromCache);
        }
    }
}
