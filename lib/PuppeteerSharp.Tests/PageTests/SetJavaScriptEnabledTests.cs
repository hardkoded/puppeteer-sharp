using System;
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

        [Test, Retry(2), PuppeteerTest("page.spec", "Page Page.setJavaScriptEnabled", "should work")]
        public async Task ShouldWork()
        {
            await Page.SetJavaScriptEnabledAsync(false);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");

            var exception = Assert.ThrowsAsync<EvaluationFailedException>(async () => await Page.EvaluateExpressionAsync("something"));
            StringAssert.Contains("something is not defined", exception.Message);

            await Page.SetJavaScriptEnabledAsync(true);
            await Page.GoToAsync("data:text/html, <script>var something = 'forbidden'</script>");
            Assert.AreEqual("forbidden", await Page.EvaluateExpressionAsync<string>("something"));
        }
    }
}
