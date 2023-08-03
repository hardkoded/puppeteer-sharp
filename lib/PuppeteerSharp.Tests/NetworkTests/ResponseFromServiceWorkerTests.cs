using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromServiceWorkerTests : PuppeteerPageBaseTest
    {
        public ResponseFromServiceWorkerTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Response.fromServiceWorker", "should return |false| for non-service-worker content")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldReturnFalseForNonServiceWorkerContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromServiceWorker);
        }

        [PuppeteerTest("network.spec.ts", "Response.fromServiceWorker", "Response.fromServiceWorker")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ResponseFromServiceWorker()
        {
            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html",
                waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            await Page.EvaluateFunctionAsync("async () => await window.activationPromise");
            await Page.ReloadAsync();

            Assert.AreEqual(2, responses.Count);
            Assert.AreEqual(HttpStatusCode.OK, responses["sw.html"].Status);
            Assert.True(responses["sw.html"].FromServiceWorker);
            Assert.AreEqual(HttpStatusCode.OK, responses["style.css"].Status);
            Assert.True(responses["style.css"].FromServiceWorker);
        }
    }
}
