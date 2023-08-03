using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.NetworkTests
{
    public class RequestPostDataTests : PuppeteerPageBaseTest
    {
        public RequestPostDataTests(): base()
        {
        }

        [PuppeteerTest("network.spec.ts", "Request.postData", "should work")]
        [Skip(SkipAttribute.Targets.Firefox)]
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

        [PuppeteerTest("network.spec.ts", "Request.postData", "should be |undefined| when there is no post data")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldBeUndefinedWhenThereIsNoPostData()
        {
            var response = await Page.GoToAsync(TestConstants.EmptyPage);
            Assert.Null(response.Request.PostData);
        }
    }
}
