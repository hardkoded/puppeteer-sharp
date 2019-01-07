using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvalTests : PuppeteerPageBaseTest
    {
        public EvalTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var idAttribute = await Page.QuerySelectorAsync("section").EvaluateFunctionAsync<string>("e => e.id");
            Assert.Equal("testAttribute", idAttribute);
        }

        [Fact]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var section = await Page.QuerySelectorAsync("section");
            var idAttribute = await section.EvaluateFunctionAsync<string>("e => e.id");
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

        [Fact]
        public async Task ShouldNotDisposeElementIfTheDisposeHandleFlagIsSetToFalse()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var section = await Page.QuerySelectorAsync("section");
            var idAttribute = await section.EvaluateFunctionAsync<string>("e => e.id", disposeHandle: false);
            var text = await section.EvaluateFunctionAsync<string>("e => e.textContent", disposeHandle: false);
            await section.DisposeAsync();
            Assert.Equal("testAttribute", idAttribute);
            Assert.Equal("43543", text);
        }

        [Fact]
        public async Task ShouldDisposeElementIfTheDisposeHandleFlagIsSetToTrue()
        {
            await Page.SetContentAsync("<section id='testAttribute'>43543</section>");
            var section = await Page.QuerySelectorAsync("section");
            await section.EvaluateFunctionAsync<string>("e => e.id", disposeHandle: true);
            var exception = await Assert.ThrowsAsync<EvaluationFailedException>(()
                => section.EvaluateFunctionAsync<string>("e => e.textContent"));
            Assert.Equal("JSHandle is disposed!", exception.Message);
        }
    }
}
