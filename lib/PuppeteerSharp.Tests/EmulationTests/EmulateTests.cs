using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.EmulationTests
{
    public class EmulateTests : PuppeteerPageBaseTest
    {
        public EmulateTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.emulate", "should work")]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/mobile.html");
            await Page.EmulateAsync(TestConstants.IPhone);

            Assert.AreEqual(375, await Page.EvaluateExpressionAsync<int>("window.innerWidth"));
            StringAssert.Contains("iPhone", await Page.EvaluateExpressionAsync<string>("navigator.userAgent"));
        }

        [Test, Retry(2), PuppeteerTest("emulation.spec", "Emulation Page.emulate", "should support clicking")]
        public async Task ShouldSupportClicking()
        {
            await Page.EmulateAsync(TestConstants.IPhone);
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/button.html");
            var button = await Page.QuerySelectorAsync("button");
            await Page.EvaluateFunctionAsync("button => button.style.marginTop = '200px'", button);
            await button.ClickAsync();
            Assert.AreEqual("Clicked", await Page.EvaluateExpressionAsync<string>("result"));
        }
    }
}
