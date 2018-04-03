using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class GotoTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldNavigateToAboutBlank()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.AboutBlank);
                Assert.Null(response);
            }
        }

        [Theory]
        [InlineData("domcontentloaded")]
        [InlineData("networkidle0")]
        [InlineData("networkidle2")]
        public async Task ShouldNavigateToEmptyPage(string waitUntil)
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { waitUntil } });
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldFailWhenNavigatingToBadUrl()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync("asdfasdf"));
                Assert.Contains("Cannot navigate to invalid URL", exception.Message);
            }
        }

        [Fact(Skip = "message is ERR_CERT_COMMON_NAME_INVALID instead of ERR_CERT_AUTHORITY_INVALID")]
        public async Task ShouldFailWhenNavigatingToBadSSL()
        {
            using (var page = await Browser.NewPageAsync())
            {
                page.RequestCreated += (sender, e) => Assert.NotNull(e.Request);
                page.RequestFinished += (sender, e) => Assert.NotNull(e.Request);
                page.RequestFailed += (sender, e) => Assert.NotNull(e.Request);

                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync(TestConstants.HttpsPrefix + "/empty.html"));
                Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
            }
        }

        [Fact(Skip = "message is ERR_CERT_COMMON_NAME_INVALID instead of ERR_CERT_AUTHORITY_INVALID")]
        public async Task ShouldFailWhenNavigatingToBadSSLAfterRedirects()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync(TestConstants.HttpsPrefix + "/redirect/2.html"));
                Assert.Contains("net::ERR_CERT_AUTHORITY_INVALID", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldThrowIfNetworkidleIsPassedAsAnOption()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync(TestConstants.EmptyPage, new NavigationOptions { WaitUntil = new[] { "networkidle" } }));
                Assert.Contains("\"networkidle\" option is no longer supported", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldFailWhenMainResourcesFailedToLoad()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync("http://localhost:44123/non-existing-url"));
                Assert.Contains("net::ERR_CONNECTION_REFUSED", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldFailWhenExceedingMaximumNavigationTimeout()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync(TestConstants.MaximumNavigationTimeout, new NavigationOptions { Timeout = 1 }));
                Assert.Contains("Navigation Timeout Exceeded: 1ms", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldFailWhenExceedingDefaultMaximumNavigationTimeout()
        {
            using (var page = await Browser.NewPageAsync())
            {
                page.DefaultNavigationTimeout = 1;
                var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.GoToAsync(TestConstants.MaximumNavigationTimeout));
                Assert.Contains("Navigation Timeout Exceeded: 1ms", exception.Message);
            }
        }

        [Fact]
        public async Task ShouldDisableTimeoutWhenItsSetTo0()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var loaded = false;
                void OnLoad(object sender, EventArgs e)
                {
                    loaded = true;
                    page.Load -= OnLoad;
                }
                page.Load += OnLoad;

                page.DefaultNavigationTimeout = 1;
                await page.GoToAsync(TestConstants.ServerUrl + "/grid.html", new NavigationOptions { Timeout = 0, WaitUntil = new[] { "load" } });
                Assert.True(loaded);
            }
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToValidUrl()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingToDataUrl()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync("data:text/html,hello");
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldWorkWhenNavigatingTo404()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.ServerUrl + "/not-found");
                Assert.Equal(HttpStatusCode.NotFound, response.Status);
            }
        }

        [Fact]
        public async Task ShouldReturnLastResponseInRedirectChain()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.ServerUrl + "/redirect/1.html");
                Assert.Equal(HttpStatusCode.OK, response.Status);
                Assert.Equal(TestConstants.EmptyPage, response.Url);
            }
        }

        [Fact(Skip = "complicated")]
        public async Task ShouldWaitForNetworkIdleToSucceedNavigation()
        {

        }

        [Fact]
        public async Task ShouldNavigateToDataURLAndFireDataURLRequests()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var requests = new List<Request>();
                page.RequestCreated += (sender, e) => requests.Add(e.Request);
                var dataUrl = "data:text/html,<div>yo</div>";
                var response = await page.GoToAsync(dataUrl);
                Assert.Equal(HttpStatusCode.OK, response.Status);
                Assert.Single(requests);
                Assert.Equal(dataUrl, requests[0].Url);
            }
        }

        [Fact]
        public async Task ShouldNavigateToURLWithHashAndFireRequestsWithoutHash()
        {
            using (var page = await Browser.NewPageAsync())
            {
                var requests = new List<Request>();
                page.RequestCreated += (sender, e) => requests.Add(e.Request);
                var response = await page.GoToAsync(TestConstants.EmptyPage + "#hash");
                Assert.Equal(HttpStatusCode.OK, response.Status);
                Assert.Equal(TestConstants.EmptyPage, response.Url);
                Assert.Single(requests);
                Assert.Equal(TestConstants.EmptyPage, requests[0].Url);
            }
        }
    }
}