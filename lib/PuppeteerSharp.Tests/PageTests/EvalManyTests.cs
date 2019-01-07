using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvalManyTests : PuppeteerPageBaseTest
    {
        public EvalManyTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("div").EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.Equal(3, divsCount);
        }

        [Fact]
        public async Task ShouldWorkWithAwaitedElements()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divs = await Page.QuerySelectorAllHandleAsync("div");
            var divsCount = await divs.EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.Equal(3, divsCount);
        }

        [Fact]
        public async Task ShouldNotDisposeArrayIfTheDisposeHandleFlagIsSetToFalse()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divs = await Page.QuerySelectorAllHandleAsync("div");
            var divsCount = await divs.EvaluateFunctionAsync<int>("divs => divs.length", disposeHandle: false);
            var text = await divs.EvaluateFunctionAsync<string>("divs => divs.map(x => x.textContent).join(' ')", disposeHandle: false);
            await divs.DisposeAsync();
            Assert.Equal(3, divsCount);
            Assert.Equal("hello beautiful world!", text);
        }

        [Fact]
        public async Task ShouldDisposeArrayIfTheDisposeHandleFlagIsSetToTrue()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divs = await Page.QuerySelectorAllHandleAsync("div");
            await divs.EvaluateFunctionAsync<int>("divs => divs.length", disposeHandle: true);
            await Assert.ThrowsAsync<MessageException>(()
                => divs.EvaluateFunctionAsync<string>("divs => divs.map(x => x.textContent).join(' ')"));
        }
    }
}
