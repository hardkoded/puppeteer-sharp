using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetExtraHttpHeadersTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            using (var page = await Browser.NewPageAsync())
            {
                await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
                {
                    ["Foo"] = "Bar"
                });

                var response = await page.GoToAsync($"{TestConstants.ServerUrl}/headertests/test");
                Assert.Equal("Bar", await response.TextAsync());
            }
        }
    }
}
