using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class QueryAllArrayTests : PuppeteerPageBaseTest
    {
        public QueryAllArrayTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryAllArray", "$$eval should handle many elements")]
        [PuppeteerFact]
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
