using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GotoTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldNavigateToAboutBlank()
        {
            var response = await Page.GoToAsync(TestConstants.AboutBlank);
            Assert.Null(response);
        }

        [Theory]
        [InlineData("domcontentloaded")]
        [InlineData("networkidle0")]
        [InlineData("networkidle2")]
        public async Task ShouldNavigateToEmptyPage(string waitUntil)
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { waitUntil } });
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadUrl()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync("asdfasdf"));
            Assert.Contains("Cannot navigate to invalid URL", exception.Message);
        }

        [Fact(Skip = "message is ERR_CERT_COMMON_NAME_INVALID instead of ERR_CERT_AUTHORITY_INVALID")]
        public async Task ShouldFailWhenNavigatingToBadSSL()
        {
            Page.RequestCreated += (sender, e) => Assert.NotNull(e.Request);
            Page.RequestFinished += (sender, e) => Assert.NotNull(e.Request);
            Page.RequestFailed += (sender, e) => Assert.NotNull(e.Request);

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));
            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [Fact(Skip = "message is ERR_CERT_COMMON_NAME_INVALID instead of ERR_CERT_AUTHORITY_INVALID")]
        public async Task ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));
            Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowIfNetworkidleIsPassedAsAnOption()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { "networkidle" } }));
            Assert.Contains("\"networkidle\" option is no longer supported", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenMainResourcesFailedToLoad()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync("http://localhost:44123/non-existing-url"));
            Assert.Contains("net::ERR_CONNECTION_REFUSED", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.MaximumNavigationTimeout, new NavigationOptions { Timeout = 1 }));
            Assert.Contains("Navigation Timeout Exceeded: 1ms", exception.Message);
        }

        [Fact]
        public async Task ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            Page.DefaultNavigationTimeout = 1;
            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.GoToAsync(TestConstants.MaximumNavigationTimeout));
            Assert.Contains("Navigation Timeout Exceeded: 1ms", exception.Message);
        }

        [Fact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            var loaded = false;
            void OnLoad(object sender, EventArgs e)
            {
                loaded = true;
                Page.Load -= OnLoad;
            }
            Page.Load += OnLoad;

            await Page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions { Timeout = 0, WaitUntil = new[] { "load" } });
            Assert.True(loaded);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            var response = await Page.GoToAsync("data:text/html,hello");
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/not-found");
            Assert.Equal(HttpStatusCode.NotFound, response.Status);
        }

        [Fact]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            var response = await Page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
        }

        [Fact(Skip = "complicated")]
        public async Task ShouldWaitForNetworkIdleToSucceedNavigation()
        {

        }

        [Fact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            var requests = new List<Request>();
            Page.RequestCreated += (sender, e) => requests.Add(e.Request);
            var dataUrl = "data:text/html,<div>yo</div>";
            var response = await Page.GoToAsync(dataUrl);
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Single(requests);
            Assert.Equal(dataUrl, requests[0].Url);
        }

        [Fact]
        public async Task ShouldNavigateToURLWithHashAndFireRequestsWithoutHash()
        {
            var requests = new List<Request>();
            Page.RequestCreated += (sender, e) => requests.Add(e.Request);
            var response = await Page.GoToAsync(TestConstants.EmptyPage + "#hash");
            Assert.Equal(HttpStatusCode.OK, response.Status);
            Assert.Equal(TestConstants.EmptyPage, response.Url);
            Assert.Single(requests);
            Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
        }
    }
}