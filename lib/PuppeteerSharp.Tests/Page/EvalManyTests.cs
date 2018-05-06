using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvalManyTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("div").EvaluateFunctionAsync<int>("divs => divs.length");
            Assert.Equal(3, divsCount);
        }
    }
}
