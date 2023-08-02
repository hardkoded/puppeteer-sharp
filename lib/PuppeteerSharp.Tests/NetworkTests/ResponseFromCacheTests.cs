using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromCacheTests : PuppeteerPageBaseTest
    {
        public ResponseFromCacheTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.fromCache", "should return |false| for non-cached content")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnFalseForNonCachedContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromCache);
        }

        [PuppeteerTest("network.spec.ts", "Response.fromCache", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldWork()
        {
            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await Page.ReloadAsync();

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.NotModified, responses["one-style.html"].Status);
            Assert.False(responses["one-style.html"].FromCache);
            Assert.Equal(HttpStatusCode.OK, responses["one-style.css"].Status);
            Assert.True(responses["one-style.css"].FromCache);
        }
    }
}
