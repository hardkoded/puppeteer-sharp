using System.Net.Http;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Helpers;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestFetchPostDataTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("network.spec", "network Request.fetchPostData", "should work for intercepted form submissions")]
        public async Task ShouldWorkForInterceptedFormSubmissions()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            await Page.SetRequestInterceptionAsync(true);
            string postData = null;
            Page.Request += async (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request) && e.Request.Method == HttpMethod.Post)
                {
                    postData = await e.Request.FetchPostDataAsync();
                }

                await e.Request.ContinueAsync();
            };

            await Task.WhenAll(
                Page.WaitForNavigationAsync(),
                Page.EvaluateFunctionAsync(@"() => {
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = '/post';
                    const input = document.createElement('input');
                    input.name = 'foo';
                    input.value = 'bar';
                    form.append(input);
                    document.body.append(form);
                    form.submit();
                }"));

            Assert.That(postData, Is.EqualTo("foo=bar"));
        }

        [Test, PuppeteerTest("network.spec", "network Request.fetchPostData", "should work for intercepted form submissions in cooperative mode")]
        public async Task ShouldWorkForInterceptedFormSubmissionsInCooperativeMode()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            await Page.SetRequestInterceptionAsync(true);
            string postData = null;
            Page.Request += async (_, e) =>
            {
                if (!TestUtils.IsFavicon(e.Request))
                {
                    await e.Request.ContinueAsync(new Payload(), 0);
                }
            };
            Page.Request += async (_, e) =>
            {
                if (e.Request.Method == HttpMethod.Post)
                {
                    postData = await e.Request.FetchPostDataAsync();
                }
            };

            await Task.WhenAll(
                Page.WaitForNavigationAsync(),
                Page.EvaluateFunctionAsync(@"() => {
                    const form = document.createElement('form');
                    form.method = 'POST';
                    form.action = '/post';
                    const input = document.createElement('input');
                    input.name = 'foo';
                    input.value = 'bar';
                    form.append(input);
                    document.body.append(form);
                    form.submit();
                }"));

            Assert.That(postData, Is.EqualTo("foo=bar"));
        }

        [Test, PuppeteerTest("network.spec", "network Request.fetchPostData", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            var requestTask = Page.WaitForRequestAsync(request => !TestUtils.IsFavicon(request));
            await Page.EvaluateExpressionHandleAsync("fetch('./post', { method: 'POST', body: JSON.stringify({ foo: 'bar'})})");
            var request = await requestTask.WithTimeout();
            Assert.That(request, Is.Not.Null);
            Assert.That(request.HasPostData, Is.True);
            Assert.That(await request.FetchPostDataAsync(), Is.EqualTo("{\"foo\":\"bar\"}"));
        }

        [Test, PuppeteerTest("network.spec", "network Request.fetchPostData", "should be |undefined| when there is no post data")]
        public async Task ShouldBeUndefinedWhenThereIsNoPostData()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.That(response.Request.HasPostData, Is.False);
            Assert.That(await response.Request.FetchPostDataAsync(), Is.Null);
        }

        [Test, PuppeteerTest("network.spec", "network Request.fetchPostData", "should work with blobs")]
        public async Task ShouldWorkWithBlobs()
        {
            await Page.GoToAsync(TestConstants.EmptyPage);
            Server.SetRoute("/post", _ => Task.CompletedTask);
            var requestTask = Page.WaitForRequestAsync(request => !TestUtils.IsFavicon(request));
            await Page.EvaluateExpressionHandleAsync(@"fetch('./post', {
                method: 'POST',
                body: new Blob([JSON.stringify({foo: 'bar'})], {
                  type: 'application/json',
                }),
            })");
            var request = await requestTask.WithTimeout();
            Assert.That(request, Is.Not.Null);
            Assert.That(request.HasPostData, Is.True);
            Assert.That(await request.FetchPostDataAsync(), Is.EqualTo("{\"foo\":\"bar\"}"));
        }
    }
}
