using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromCacheTests : PuppeteerPageBaseTest
    {
        public ResponseFromCacheTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "Response.fromCache", "should return |false| for non-cached content")]
        public async Task ShouldReturnFalseForNonCachedContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromCache);
        }

        [Test, PuppeteerTest("network.spec", "Response.fromCache", "should work")]
        public async Task ShouldWork()
        {
            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await Page.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await Page.ReloadAsync();

            Assert.AreEqual(2, responses.Count);
            Assert.AreEqual(HttpStatusCode.NotModified, responses["one-style.html"].Status);
            Assert.False(responses["one-style.html"].FromCache);
            Assert.AreEqual(HttpStatusCode.OK, responses["one-style.css"].Status);
            Assert.True(responses["one-style.css"].FromCache);
        }
    }
}
