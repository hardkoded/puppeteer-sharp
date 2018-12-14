using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetContentTests : PuppeteerPageBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        public SetContentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal(ExpectedOutput, result);
        }

        [Fact]
        public async Task ShouldWorkWithDoctype()
        {
            const string doctype = "<!DOCTYPE html>";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact]
        public async Task ShouldAwaitResourceToLoad()
        {
            var imgPath = "/img.png";
            var imgResponse = new TaskCompletionSource<bool>();
            Server.SetRoute(imgPath, context => imgResponse.Task);
            var loaded = false;
            var waitTask = Server.WaitForRequest(imgPath);
            var contentTask = Page.SetContentAsync($"<img src=\"{TestConstants.ServerUrl + imgPath}\"></img>")
                .ContinueWith(_ => loaded = true);
            await waitTask;
            Assert.False(loaded);
            imgResponse.SetResult(true);
            await contentTask;
        }
    }
}
