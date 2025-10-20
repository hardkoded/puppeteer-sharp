using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class HoverTests : PuppeteerPageBaseTest
    {
        public HoverTests() : base()
        {
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs ElementHandle.hover", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            var button = await Page.QuerySelectorAsync("#button-6");
            await button.HoverAsync();
            Assert.That(await Page.EvaluateExpressionAsync<string>(
                "document.querySelector('button:hover').id"), Is.EqualTo("button-6"));
        }
    }
}
