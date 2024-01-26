using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class HoverTests : PuppeteerPageBaseTest
    {
        public HoverTests() : base()
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "ElementHandle.hover", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/input/scrollable.html");
            var button = await Page.QuerySelectorAsync("#button-6");
            await button.HoverAsync();
            Assert.AreEqual("button-6", await Page.EvaluateExpressionAsync<string>(
                "document.querySelector('button:hover').id"));
        }
    }
}
