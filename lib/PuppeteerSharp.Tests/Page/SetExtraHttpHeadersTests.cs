using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetExtraHttpHeadersTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["Foo"] = "Bar"
            });

            var response = await Page.GoToAsync($"{TestConstants.ServerUrl}/headertests/test");
            Assert.Equal("Bar", await response.TextAsync());
        }
    }
}
