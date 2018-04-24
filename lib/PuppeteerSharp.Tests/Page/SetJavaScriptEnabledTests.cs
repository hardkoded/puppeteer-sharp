using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetJavaScriptEnabledTests : PuppeteerPageBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await Page.EvaluateExpressionAsync("something"));
            Assert.Contains("something is not defined", exception.Message);

            await Page.SetJavaScriptEnabledAsync(true);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.Equal("forbidden", await Page.EvaluateExpressionAsync<string>("something"));
        }
    }
}