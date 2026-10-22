using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestContentTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.content", "should work with binary post data")]
        public async Task ShouldWorkWithBinaryPostData()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            var requestTask = Page.WaitForRequestAsync(request => !TestUtils.IsFavicon(request));
            await Page.EvaluateExpressionHandleAsync("fetch('./post', { method: 'POST', body: new Uint8Array([0, 255, 128, 10, 200, 1]) })");
            var request = await requestTask.WithTimeout();
            Assert.That(request, Is.Not.Null);
            Assert.That(request.HasPostData, Is.True);
            Assert.That(await request.ContentAsync(), Is.EqualTo(new byte[] { 0, 255, 128, 10, 200, 1 }));
        }

        [Test, PuppeteerTest("network.spec", "network Request.content", "should be |undefined| when there is no post data")]
        public async Task ShouldBeUndefinedWhenThereIsNoPostData()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Request.HasPostData, Is.False);
            Assert.That(await response.Request.ContentAsync(), Is.Null);
        }
    }
}
