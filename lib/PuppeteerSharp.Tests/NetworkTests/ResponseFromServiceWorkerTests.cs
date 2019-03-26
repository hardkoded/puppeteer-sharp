using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.NetworkTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class ResponseFromServiceWorkerTests : PuppeteerPageBaseTest
    {
        public ResponseFromServiceWorkerTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldReturnFalseForNonServiceWorkerContent()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.False(response.FromServiceWorker);
        }

        [Fact]
        public async Task ResponseFromServiceWorker()
        {
            var responses = new Dictionary<string, Response>();
            Page.Response += (sender, e) => responses[e.Response.Url.Split('/').Last()] = e.Response;
            await Page.GoToAsync(TestConstants.ServerUrl + "/serviceworkers/fetch/sw.html",
                waitUntil: new[] { WaitUntilNavigation.Networkidle2 });
            await Page.EvaluateFunctionAsync("async () => await window.activationPromise");
            await Page.ReloadAsync();

            Assert.Equal(2, responses.Count);
            Assert.Equal(HttpStatusCode.OK, responses["sw.html"].Status);
            Assert.True(responses["sw.html"].FromServiceWorker);
            Assert.Equal(HttpStatusCode.OK, responses["style.css"].Status);
            Assert.True(responses["style.css"].FromServiceWorker);
        }
    }
}
