using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromCacheTests : PuppeteerPageBaseTest
    {
        public ResponseFromCacheTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Response.fromCache", "should return |false| for non-cached content")]
        public async Task ShouldReturnFalseForNonCachedContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.FromCache, Is.False);
        }

        [Test, PuppeteerTest("network.spec", "network Response.fromCache", "should work")]
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

            Assert.That(responses, Has.Count.EqualTo(2));
            Assert.That(responses["one-style.html"].Status, Is.EqualTo(HttpStatusCode.NotModified));
            Assert.That(responses["one-style.html"].FromCache, Is.False);
            Assert.That(responses["one-style.css"].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses["one-style.css"].FromCache, Is.True);
        }
    }
}
