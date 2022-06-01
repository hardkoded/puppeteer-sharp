using System;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.DevToolsContextTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class SetJavaScriptEnabledTests : PuppeteerPageBaseTest
    {
        public SetJavaScriptEnabledTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("page.spec.ts", "Page.setJavaScriptEnabled", "should work")]
        public async Task ShouldWork()
        {
            await DevToolsContext.SetJavaScriptEnabledAsync(false);
            await DevToolsContext.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = await Assert.ThrowsAnyAsync<Exception>(async () => await DevToolsContext.EvaluateExpressionAsync("something"));
            Assert.Contains("something is not defined", exception.Message);

            await DevToolsContext.SetJavaScriptEnabledAsync(true);
            await DevToolsContext.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.Equal("forbidden", await DevToolsContext.EvaluateExpressionAsync<string>("something"));
        }
    }
}
