using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AuthenticateTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var response = await Page.GoToAsync(TestConstants.AuthenticateUrl);
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);

            await Page.AuthenticateAsync(new Credentials
            {
                Username = "user",
                Password = "pass"
            });

            response = await Page.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [Fact]
        public async Task ShouldFailIfWrongCredentials()
        {
            await Page.AuthenticateAsync(new Credentials
            {
                Username = "foo",
                Password = "bar"
            });

            var response = await Page.GoToAsync(TestConstants.AuthenticateUrl);
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
        }

        [Fact]
        public async Task ShouldAllowDisableAuthentication()
        {
            await Page.AuthenticateAsync(new Credentials
            {
                Username = "user",
                Password = "pass"
            });

            var response = await Page.GoToAsync(TestConstants.AuthenticateUrl);
            Assert.Equal(HttpStatusCode.OK, response.Status);

            await Page.AuthenticateAsync(null);

            response = await Page.GoToAsync(TestConstants.CrossProcessAuthenticateUrl);
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
        }
    }
}
