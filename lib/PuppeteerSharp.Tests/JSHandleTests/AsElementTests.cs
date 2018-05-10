﻿using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class AsElementTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.body");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
        }

        [Fact]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("2");
            var element = aHandle as ElementHandle;
            Assert.Null(element);
        }

        [Fact]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await Page.SetContentAsync("<div>ee!</div>");
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.querySelector('div').firstChild");
            var element = aHandle as ElementHandle;
            Assert.NotNull(element);
            Assert.NotNull(await Page.EvaluateFunctionAsync("e => e.nodeType === HTMLElement.TEXT_NODE", element));
        }
    }
}