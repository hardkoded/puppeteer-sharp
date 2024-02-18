using System.Threading.Tasks;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.Issues
{
    public class Issue0764 : PuppeteerPageBaseTest
    {
        public Issue0764() : base()
        {
        }

        public async Task BufferAsyncShouldWorkWithBinaries()
        {
            var tcs = new TaskCompletionSource<byte[]>();
            Page.Response += async (_, e) =>
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
