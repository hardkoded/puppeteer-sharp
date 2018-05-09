﻿using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvalTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var idAttribute = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id");
            Assert.Equal("testAttribute", idAttribute);
        }

        [Fact]
        public async Task ShouldAcceptArguments()
        {
            await Page.SetContentAsync("<section>hello</section>");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, suffix) => e.textContent + suffix", " world!");
            Assert.Equal("hello world!", text);
        }

        [Fact]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>hello</section><div> world</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var text = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("(e, div) => e.textContent + div.textContent", divHandle);
            Assert.Equal("hello world", text);
        }

        [Fact]
        public async Task ShouldThrowErrorIfNoElementIsFound()
        {
            var exception = await Assert.ThrowsAsync<SelectorException>(()
                => Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id"));
            Assert.Contains("failed to find element matching selector", exception.Message);
        }
    }
}
