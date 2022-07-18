using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextAuthenticateTests : DevToolsContextBaseTest
    {
        public DevToolsContextAuthenticateTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Page.authenticate", "should work")]
        [PuppeteerFact]
        public async Task ShouldWork()
        {
            Server.SetAuth("/empty.html", "user", "pass");

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);

            await DevToolsContext.AuthenticateAsync(new Credentials
            {
                Username = "user",
                Password = "pass"
            });

            response = await DevToolsContext.ReloadAsync();
            Assert.Equal(HttpStatusCode.OK, response.Status);
        }

        [PuppeteerTest("network.spec.ts", "Page.authenticate", "should fail if wrong credentials")]
        [PuppeteerFact]
        public async Task ShouldFailIfWrongCredentials()
        {
            Server.SetAuth("/empty.html", "user2", "pass2");

            await DevToolsContext.AuthenticateAsync(new Credentials
            {
                Username = "foo",
                Password = "bar"
            });

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
        }

        [PuppeteerTest("network.spec.ts", "Page.authenticate", "should allow disable authentication")]
        [PuppeteerFact]
        public async Task ShouldAllowDisableAuthentication()
        {
            Server.SetAuth("/empty.html", "user3", "pass3");

            await DevToolsContext.AuthenticateAsync(new Credentials
            {
                Username = "user3",
                Password = "pass3"
            });

            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.Equal(HttpStatusCode.OK, response.Status);

            await DevToolsContext.AuthenticateAsync(null);

            response = await DevToolsContext.GoToAsync(TestConstants.CrossProcessUrl + "/empty.html");
            Assert.Equal(HttpStatusCode.Unauthorized, response.Status);
        }

        [PuppeteerTest("network.spec.ts", "Page.authenticate", "should not disable caching")]
        [PuppeteerFact(Skip = "TODO: NOT WORKING IN CEF")]
        public async Task ShouldNotDisableCaching()
        {
            Server.SetAuth("/cached/one-style.css", "user4", "pass4");
            Server.SetAuth("/cached/one-style.html", "user4", "pass4");

            await DevToolsContext.AuthenticateAsync(new Credentials
            {
                Username = "user4",
                Password = "pass4"
            });

            var responses = new Dictionary<string, Response>();
            DevToolsContext.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await DevToolsContext.ReloadAsync();

            Assert.Equal(HttpStatusCode.NotModified, responses["one-style.html"].Status);
            Assert.False(responses["one-style.html"].FromCache);
            Assert.Equal(HttpStatusCode.OK, responses["one-style.css"].Status);
            Assert.True(responses["one-style.css"].FromCache);
        }
    }
}
