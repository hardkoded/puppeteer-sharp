using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.JSHandleTests
{
    public class AsElementTests : PuppeteerPageBaseTest
    {
        public AsElementTests() : base()
        {
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should work")]
        [PuppeteerTimeout]
        public async Task ShouldWork()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.body");
            var element = aHandle as IElementHandle;
            Assert.NotNull(element);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return null for non-elements")]
        [PuppeteerTimeout]
        public async Task ShouldReturnNullForNonElements()
        {
            var aHandle = await Page.EvaluateExpressionHandleAsync("2");
            var element = aHandle as IElementHandle;
            Assert.Null(element);
        }

        [PuppeteerTest("jshandle.spec.ts", "JSHandle.asElement", "should return ElementHandle for TextNodes")]
        [PuppeteerTimeout]
        public async Task ShouldReturnElementHandleForTextNodes()
        {
            await Page.SetContentAsync("<div>ee!</div>");
            var aHandle = await Page.EvaluateExpressionHandleAsync("document.querySelector('div').firstChild");
            var element = aHandle as IElementHandle;
            Assert.NotNull(element);
            Assert.True(await Page.EvaluateFunctionAsync<bool>("e => e.nodeType === HTMLElement.TEXT_NODE", element));
        }
    }
}
