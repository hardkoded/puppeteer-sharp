using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AuthenticateTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            //server.setAuth('/empty.html', 'user', 'pass');
            using (var page = await Browser.NewPageAsync())
            {
                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.Status);

                await page.AuthenticateAsync(new Credentials
                {
                    Username = "user",
                    Password = "pass"
                });

                await page.ReloadAsync();
                Assert.Equal(HttpStatusCode.OK, response.Status);
            }
        }

        [Fact]
        public async Task ShouldFailIfWrongCredentials()
        {
            //server.setAuth('/empty.html', 'user2', 'pass2');
            using (var page = await Browser.NewPageAsync())
            {
                await page.AuthenticateAsync(new Credentials
                {
                    Username = "foo",
                    Password = "bar"
                });

                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
            }
        }

        [Fact]
        public async Task ShouldAllowDisableAuthentication()
        {
            //server.setAuth('/empty.html', 'user3', 'pass3');
            using (var page = await Browser.NewPageAsync())
            {
                await page.AuthenticateAsync(new Credentials
                {
                    Username = "user3",
                    Password = "pass3"
                });

                var response = await page.GoToAsync(TestConstants.EmptyPage);
                Assert.Equal(HttpStatusCode.OK, response.Status);

                await page.AuthenticateAsync(null);

                response = await page.GoToAsync(TestConstants.CrossProcessEmptyPage);
                Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
            }
        }
    }
}
