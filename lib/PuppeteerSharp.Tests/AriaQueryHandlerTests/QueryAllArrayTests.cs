using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class QueryAllArrayTests : PuppeteerPageBaseTest
    {
        public QueryAllArrayTests() : base()
        {
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "queryAllArray", "$$eval should handle many elements")]
        public async Task EvalShouldHandleManyElements()
        {
            await Page.SetContentAsync("");
            await Page.EvaluateExpressionAsync(@"
                for (var i = 0; i <= 10000; i++) {
                    const button = document.createElement('button');
                    button.textContent = i;
                    document.body.appendChild(button);
                }
            ");
            var sum = await Page
                .QuerySelectorAllHandleAsync("aria/[role=\"button\"]")
                .EvaluateFunctionAsync<int>(@"buttons => buttons.reduce((acc, button) => acc + Number(button.textContent), 0)");

            Assert.That(sum, Is.EqualTo(50005000));
        }
    }
}
