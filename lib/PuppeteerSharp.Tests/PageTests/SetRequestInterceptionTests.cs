using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetRequestInterceptionTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldIntercept()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Contains("empty.html", e.Request.Url);
                Assert.NotNull(e.Request.Headers);
                Assert.Equal(HttpMethod.Get, e.Request.Method);
                Assert.Null(e.Request.PostData);
                Assert.Equal(ResourceType.Document, e.Request.ResourceType);
                Assert.Equal(Page.MainFrame, e.Request.Frame);
                Assert.Equal("about:blank", e.Request.Frame.Url);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldStopIntercepting()
        {
            await Page.SetRequestInterceptionAsync(true);
            async void EventHandler(object sender, RequestEventArgs e)
            {
                await e.Request.ContinueAsync();
                Page.RequestCreated -= EventHandler;
            }
            Page.RequestCreated += EventHandler;
            await Page.GoToAsync(TestConstants.EmptyPage);
            await Page.SetRequestInterceptionAsync(false);
            await Page.GoToAsync(TestConstants.EmptyPage);
        }

        [Fact]
        public async Task ShouldShowCustomHTTPHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["foo"] = "bar"
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Equal("bar", e.Request.Headers["foo"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldWorksWithCustomizingRefererHeaders()
        {
            await Page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
            {
                ["referer"] = TestConstants.EmptyPage
            });
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                Assert.Equal(TestConstants.EmptyPage, e.Request.Headers["referer"]);
                await e.Request.ContinueAsync();
            };
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.True(response.Ok);
        }

        [Fact]
        public async Task ShouldBeAbortable()
        {
            await Page.SetRequestInterceptionAsync(true);
            Page.RequestCreated += async (sender, e) =>
            {
                if (e.Request.Url.EndsWith(".css"))
                {
                    await e.Request.AbortAsync();
                }
                else
                {
                    await e.Request.ContinueAsync();
                }
            };
            var failedRequests = 0;
            Page.RequestFailed += (sender, e) => failedRequests++;
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/one-style.html");
            Assert.True(response.Ok);
            Assert.Null(response.Request.Failure);
            Assert.Equal(1, failedRequests);
        }

        [Fact]
        public async Task ShouldBeAbortableWithCustomErrorCodes()
        {

        }

        [Fact]
        public async Task ShouldAmendHTTPHeaders()
        {

        }

        [Fact]
        public async Task ShouldFailNavigationWhenAbortingMainResource()
        {

        }

        [Fact]
        public async Task ShouldWorkWithRedirects()
        {

        }

        [Fact]
        public async Task ShouldBeAbleToAbortRedirects()
        {

        }

        [Fact]
        public async Task ShouldWorkWithEqualRequests()
        {

        }

        [Fact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {

        }

        [Fact]
        public async Task ShouldAbortDataServer()
        {

        }

        [Fact]
        public async Task ShouldNavigateToURLWithHashAndAndFireRequestsWithoutHash()
        {

        }

        [Fact]
        public async Task ShouldWorkWithEncodedServer()
        {

        }

        [Fact]
        public async Task ShouldWorkWithBadlyEncodedServer()
        {

        }

        [Fact]
        public async Task ShouldWorkWithEncodedServerNegative2()
        {

        }

        [Fact]
        public async Task ShouldNotThrowInvalidInterceptionIdIfTheRequestWasCancelled()
        {

        }

        [Fact]
        public async Task ShouldThrowIfInterceptionIsNotEnabled()
        {

        }
    }
}
