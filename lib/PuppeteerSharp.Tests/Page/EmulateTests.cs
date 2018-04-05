using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EmulateTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var iPhone = DeviceDescriptors.Get(DeviceDescriptorName.IPhone6);

            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            await Page.EmulateAsync(iPhone);

            Assert.Equal(375, await Page.EvaluateFunctionAsync<int>("() => window.innerWidth"));
            Assert.Contains("Safari", await Page.EvaluateFunctionAsync<string>("() => navigator.userAgent"));
        }

        [Fact]
        public async Task ShouldSupportClicking()
        {
            var iPhone = DeviceDescriptors.Get(DeviceDescriptorName.IPhone6);

            await Page.EmulateAsync(iPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.GetElementAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.marginTop = '200px'", button);
            await button.ClickAsync();
            Assert.Equal("Clicked", await Page.EvaluateFunctionAsync("() => result"));
        }
    }
}
