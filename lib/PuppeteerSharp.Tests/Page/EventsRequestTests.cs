using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EventsRequestTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldFire()
        {
            List<Request> requests = new List<Request>();
            Page.RequestCreated += (sender, eventArgs) => requests.Add(eventArgs.Request);

            await Page.GoToAsync(TestConstants.EmptyPage);
            await FrameUtils.AttachFrame(Page, "frame1", TestConstants.EmptyPage);

            Assert.Equal(2, requests.Count);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            Assert.Equal(Page.MainFrame, requests[0].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Frame.Url);

            Assert.Equal(TestConstants.EmptyPage, requests[1].Url);
            Assert.Equal(Page.Frames.ElementAt(1), requests[1].Frame);
            Assert.Equal(TestConstants.EmptyPage, requests[1].Frame.Url);
        }
    }
}
