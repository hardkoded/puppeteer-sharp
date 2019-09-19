using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetJavaScriptEnabledTests : PuppeteerPageBaseTest
    {
        public SetJavaScriptEnabledTests(ITestOutputHelper output) : base(output)
        {
        }

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