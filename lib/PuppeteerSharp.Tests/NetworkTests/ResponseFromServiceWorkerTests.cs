using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class ResponseFromServiceWorkerTests : PuppeteerPageBaseTest
    {
        public ResponseFromServiceWorkerTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Response.fromServiceWorker", "should return |false| for non-service-worker content")]
        public async Task ShouldReturnFalseForNonServiceWorkerContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.FromServiceWorker, Is.False);
        }

        [Test, PuppeteerTest("network.spec", "network Response.fromServiceWorker", "Response.fromServiceWorker")]
        public async Task ResponseFromServiceWorker()
        {
            var responses = new Dictionary<string, IResponse>();
            Page.Response += (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Response.Request))
                {
                    responses[e.Response.Url.Split('/').Last()] = e.Response;
                }
            };
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html",
                waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            await Page.EvaluateFunctionAsync("async () => await window.activationPromise");
            await Page.ReloadAsync();

            Assert.That(responses, Has.Count.EqualTo(2));
            Assert.That(responses["sw.html"].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses["sw.html"].FromServiceWorker, Is.True);
            Assert.That(responses["style.css"].Status, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responses["style.css"].FromServiceWorker, Is.True);
        }
    }
}
