using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageEventRequestTests : PuppeteerPageBaseTest
    {
        public PageEventRequestTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldFireForNavigationRequests()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Single(requests);
        }

        [Fact]
        public async Task ShouldFireForIframes()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);

            await FrameUtils.AttachFrameAsync(Page, "frame1", TestConstants.EmptyPage);
            Assert.Equal(2, requests.Count);
        }

        [Fact]
        public async Task ShouldFireForFetches()
        {
            var requests = new List<Request>();
            Page.Request += (sender, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    requests.Add(e.Request);
                }
            };

            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.EvaluateExpressionAsync("fetch('/empty.html')");
            Assert.Equal(2, requests.Count);
        }
    }
}
