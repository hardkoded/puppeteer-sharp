using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestPostDataTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.postData", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            IRequest request = null;
            Page.Request += (_, e) => request = e.Request;
            await Page.EvaluateExpressionHandleAsync("fetch('./post', { method: 'POST', body: JSON.stringify({ foo: 'bar'})})");
            Assert.NotNull(request);
            Assert.AreEqual("{\"foo\":\"bar\"}", request.PostData);
        }

        [Test, PuppeteerTest("network.spec", "network Request.postData", "should be |undefined| when there is no post data")]
        public async Task ShouldBeUndefinedWhenThereIsNoPostData()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Null(response.Request.PostData);
        }

        [Test, PuppeteerTest("network.spec", "network Request.postData", "should work with blobs")]
        public async Task ShouldWorkWithBlobs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            IRequest request = null;
            Page.Request += (_, e) => request = e.Request;
            await Page.EvaluateExpressionHandleAsync(@"fetch('./post', {
                method: 'POST',
                body:new Blob([JSON.stringify({foo: 'bar'})], {
                  type: 'application/json',
                }),
            })");
            Assert.NotNull(request);
            Assert.Null(request.PostData);
            Assert.True(request.HasPostData);
            Assert.AreEqual("{\"foo\":\"bar\"}", await request.FetchPostDataAsync());
        }
    }
}
