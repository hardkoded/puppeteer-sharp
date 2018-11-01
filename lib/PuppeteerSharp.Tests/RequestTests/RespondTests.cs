using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.RequestTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class RespondTests : PuppeteerPageBaseTest
    {
        public RespondTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                await e.Request.RespondAsync(new ResponseData
                {
                    Status = HttpStatusCode.Created,
                    Headers = new Dictionary<string, object>
                    {
                        ["foo"] = "bar"
                    },
                    Body = "Yo, page!"
                });
            };

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.Created, response.Status);
            Assert.Equal("bar", response.Headers["foo"]);
            Assert.Equal("Yo, page!", await Page.EvaluateExpressionAsync<string>("document.body.textContent"));
        }

        [Fact]
        public async Task ShouldAllowMockingBinaryResponses()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.Request += async (sender, e) =>
            {
                var imageData = File.ReadAllBytes("./Assets/pptr.png");
                await e.Request.RespondAsync(new ResponseData
                {
                    ContentType = "image/png",
                    BodyData = imageData
                });
            };

            await Page.EvaluateFunctionAsync(@"PREFIX =>
            {
                const img = document.createElement('img');
                img.src = PREFIX + '/does-not-exist.png';
                document.body.appendChild(img);
                return new Promise(fulfill => img.onload = fulfill);
            }", TestConstants.ServerUrl);
            var img = await Page.QuerySelectorAsync("img");
            Assert.True(ScreenshotHelper.PixelMatch("mock-binary-response.png", await img.ScreenshotDataAsync()));
        }
    }
}
