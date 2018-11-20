using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.Issues
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class Issue0764 : PuppeteerPageBaseTest
    {
        public Issue0764(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task BufferAsyncShouldWorkWithBinaries()
        {
            var tcs = new TaskCompletionSource<byte[]>();
            Page.Response += async (sender, e) =>
            {
                if (e.Response.Url.Contains("digits/0.png"))
                {
                    tcs.TrySetResult(await e.Response.BufferAsync());
                }
            };

            await Task.WhenAll(
                Page.GoToAsync(TestConstants.ServerUrl + "/grid.html"),
                tcs.Task);
            Assert.True(ScreenshotHelper.PixelMatch("0.png", await tcs.Task));
        }
    }
}