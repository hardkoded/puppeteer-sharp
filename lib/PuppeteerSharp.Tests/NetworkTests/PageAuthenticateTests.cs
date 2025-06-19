using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class PageAuthenticateTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Page.authenticate", "should work")]
        public async Task ShouldWork()
        {
            Server.SetAuth("/empty.html", "user", "pass");

            try
            {
                var response = await Page.GoToAsync(TestConstants.EmptyPage);
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("net::ERR_INVALID_AUTH_CREDENTIALS"))
                {
                    throw;
                }
            }

            await Page.AuthenticateAsync(new Credentials
            {
                Username = "user",
                Password = "pass"
            });

            var newResponse = await Page.ReloadAsync();
            Assert.That(newResponse.Status, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, PuppeteerTest("network.spec", "network Page.authenticate", "should fail if wrong credentials")]
        public async Task ShouldFailIfWrongCredentials()
        {
            Server.SetAuth("/empty.html", "user2", "pass2");

            await Page.AuthenticateAsync(new Credentials
            {
                Username = "foo",
                Password = "bar"
            });

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test, PuppeteerTest("network.spec", "network Page.authenticate", "should allow disable authentication")]
        public async Task ShouldAllowDisableAuthentication()
        {
            Server.SetAuth("/empty.html", "user3", "pass3");

            await Page.AuthenticateAsync(new Credentials
            {
                Username = "user3",
                Password = "pass3"
            });

            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Status, Is.EqualTo(HttpStatusCode.OK));

            await Page.AuthenticateAsync(new Credentials()
            {
                Username = "",
                Password = ""
            });

            try
            {
                response = await Page.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
                Assert.That(response.Status, Is.EqualTo(HttpStatusCode.Unauthorized));
            }
            catch (Exception e)
            {
                if (!e.Message.Contains("net::ERR_INVALID_AUTH_CREDENTIALS"))
                {
                    throw;
                }
            }
        }

        [Test, PuppeteerTest("network.spec", "network Page.authenticate", "should not disable caching")]
        public async Task ShouldNotDisableCaching()
        {
            Server.SetAuth("/cached/one-style.css", "user4", "pass4");
            Server.SetAuth("/cached/one-style.html", "user4", "pass4");

            await Page.AuthenticateAsync(new Credentials
            {
                Username = "user4",
                Password = "pass4"
            });

            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;

            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await Page.ReloadAsync();

            Assert.That(responses["one-style.html"].Status, Is.EqualTo(HttpStatusCode.NotModified));
            Assert.That(responses["one-style.html"].FromCache, Is.False);
            Assert.That(responses["one-style.css"].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses["one-style.css"].FromCache, Is.True);
        }
    }
}
