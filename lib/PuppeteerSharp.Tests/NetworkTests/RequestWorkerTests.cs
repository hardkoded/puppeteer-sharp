using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestWorkerTests : PuppeteerPageBaseTest
    {
        public RequestWorkerTests() : base()
        {
        }

        [Test, PuppeteerTest("network.spec", "network Request.worker", "should return null for page navigation requests")]
        public async Task ShouldReturnNullForPageNavigationRequests()
        {
            var requestTask = Page.WaitForRequestAsync(TestConstants.EmptyPage);
            await Page.GoToAsync(TestConstants.EmptyPage);
            var request = await requestTask;
            Assert.That(request.Worker, Is.Null);
        }
    }
}
