using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.FrameTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GoToTests : PuppeteerPageBaseTest
    {
        public GoToTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldNavigateSubFrames()
        {
            //var response = await Page.GoToAsync("data:text/html,hello");
            await Page.GoToAsync(TestConstants.ServerUrl + "/frames/one-frame.html");
            Assert.Contains("/frames/one-frame.html", Page.Frames[0].Url);
            Assert.Contains("/frames/frame.html", Page.Frames[1].Url);
            var response = await Page.Frames[1].GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }
    }
}
