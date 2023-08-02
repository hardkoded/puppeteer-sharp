using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class QueryAllArrayTests : PuppeteerPageBaseTest
    {
        public QueryAllArrayTests(): base()
        {
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryAllArray", "$$eval should handle many elements")]
        [SkipBrowserFact(skipFirefox: true)]
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

            Assert.Equal(50005000, sum);
        }
    }
}
