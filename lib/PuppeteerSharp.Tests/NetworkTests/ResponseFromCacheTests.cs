using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromCacheTests : PuppeteerPageBaseTest
    {
        public ResponseFromCacheTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.fromCache", "should return |false| for non-cached content")]
        public async Task ShouldReturnFalseForNonCachedContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromCache);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Response.fromCache", "should work")]
        public async Task ShouldWork()
        {
            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    responses[e.Response.Url.Split('/').Last()] = e.Response;
                }
            };
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
