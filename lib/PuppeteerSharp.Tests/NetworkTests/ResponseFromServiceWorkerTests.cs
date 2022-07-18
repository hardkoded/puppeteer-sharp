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
    public class ResponseFromServiceWorkerTests : DevToolsContextBaseTest
    {
        public ResponseFromServiceWorkerTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.fromServiceWorker", "should return |false| for non-service-worker content")]
        [PuppeteerFact]
        public async Task ShouldReturnFalseForNonServiceWorkerContent()
        {
            var response = await DevToolsContext.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromServiceWorker);
        }

        [PuppeteerTest("network.spec.ts", "Response.fromServiceWorker", "Response.fromServiceWorker")]
        [PuppeteerFact]
        public async Task ResponseFromServiceWorker()
        {
            var responses = new Dictionary<string, Response>();
            DevToolsContext.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html",
                waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            await DevToolsContext.EvaluateFunctionAsync("async () => await window.activationPromise");
            await DevToolsContext.ReloadAsync();

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.OK, responses["sw.html"].Status);
            Assert.True(responses["sw.html"].FromServiceWorker);
            Assert.Equal(HttpStatusCode.OK, responses["style.css"].Status);
            Assert.True(responses["style.css"].FromServiceWorker);
        }
    }
}
