using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestPostDataTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.postData", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            var requestTask = Page.WaitForRequestAsync((request) => !TestUtils.IsFavicon(request));
            await Page.EvaluateExpressionHandleAsync("fetch('./post', { method: 'POST', body: JSON.stringify({ foo: 'bar'})})");
            var request = await requestTask.WithTimeout();
            Assert.NotNull(request);
            Assert.AreEqual("{\"foo\":\"bar\"}", request.PostData);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.postData", "should be |undefined| when there is no post data")]
        public async Task ShouldBeUndefinedWhenThereIsNoPostData()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Null(response.Request.PostData);
        }

        [Test, Retry(2), PuppeteerTest("network.spec", "network Request.postData", "should work with blobs")]
        public async Task ShouldWorkWithBlobs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            var requestTask = Page.WaitForRequestAsync((request) => !TestUtils.IsFavicon(request));
            await Page.EvaluateExpressionHandleAsync(@"fetch('./post', {
                method: 'POST',
                body:new Blob([JSON.stringify({foo: 'bar'})], {
                  type: 'application/json',
                }),
            })");
            var request = await requestTask.WithTimeout();
            Assert.NotNull(request);
            Assert.Null(request.PostData);
            Assert.True(request.HasPostData);
            Assert.AreEqual("{\"foo\":\"bar\"}", await request.FetchPostDataAsync());
        }
    }
}
