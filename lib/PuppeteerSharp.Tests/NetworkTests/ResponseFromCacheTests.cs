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
    public class ResponseFromCacheTests : DevToolsContextBaseTest
    {
        public ResponseFromCacheTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.fromCache", "should return |false| for non-cached content")]
        [PuppeteerFact]
        public async Task ShouldReturnFalseForNonCachedContent()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromCache);
        }

        [PuppeteerTest("network.spec.ts", "Response.fromCache", "should work")]
        [PuppeteerFact(Skip = "TODO: CEF")]
        public async Task ShouldWork()
        {
            var responses = new Dictionary<string, Response>();
            DevToolsContext.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/cached/one-style.html");
            await DevToolsContext.ReloadAsync();

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.NotModified, responses["one-style.html"].Status);
            Assert.False(responses["one-style.html"].FromCache);
            Assert.Equal(HttpStatusCode.OK, responses["one-style.css"].Status);
            Assert.True(responses["one-style.css"].FromCache);
        }
    }
}
