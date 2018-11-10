using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests.Events
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class RequestTests : PuppeteerPageBaseTest
    {
        public RequestTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFire()
        {
            List<Request> requests = new List<Request>();
            Page.Request += (sender, e) => requests.Add(e.Request);

            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);

            Assert.Equal(2, requests.Count);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);

            Assert.Equal(TestConstants.EmptyPage, requests[1].Url);
            Assert.Equal(Page.FirstChildFrame(), requests[1].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[1].Frame.Url);
        }
    }
}
