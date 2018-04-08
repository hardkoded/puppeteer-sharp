using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetUserAgentTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            Assert.Contains("Mozilla", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
            await Page.SetUserAgentAsync("foobar");

            // todo: make like puppeteer
            Assert.Equal("foobar", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
        }
    }
}
