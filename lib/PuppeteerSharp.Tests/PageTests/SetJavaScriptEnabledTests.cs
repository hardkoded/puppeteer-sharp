using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class SetJavaScriptEnabledTests : PuppeteerPageBaseTest
    {
        public SetJavaScriptEnabledTests() : base()
        {
        }

        [Test, PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.EvaluateExpressionAsync("something"));
            Assert.That(exception.Message, Does.Contain("something is not defined"));

            await Page.SetJavaScriptEnabledAsync(true);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.That(await Page.EvaluateExpressionAsync<string>("something"), Is.EqualTo("forbidden"));
        }
    }
}
