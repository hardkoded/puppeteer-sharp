using System;
using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.Page
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetJavaScriptEnabledTests : PuppeteerBaseTest
    {
        [Fact]
        public async Task ShouldWork()
        {
            var page = await Browser.NewPageAsync();
            await page.SetJavaScriptEnabledAsync(false);
            await page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await page.EvaluateFunctionAsync("something"));
            Assert.Contains("something is not defined", exception.Message);

            await page.SetJavaScriptEnabledAsync(true);
            await page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.Equal("forbidden", await page.EvaluateExpressionAsync<string>("something"));
        }
    }
}
