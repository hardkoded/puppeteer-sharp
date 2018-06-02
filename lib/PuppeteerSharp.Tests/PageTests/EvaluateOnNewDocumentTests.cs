using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class EvaluateOnNewDocumentTests : PuppeteerPageBaseTest
    {
        public EvaluateOnNewDocumentTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public async Task ShouldEvaluateBeforeAnythingElseOnThePage()
        {
            await Page.EvaluateOnNewDocumentAsync(@"function(){
                window.injected = 123;
            }");
            await Page.GoToAsync(TestConstants.ServerUrl + "/tamperable.html");
            Assert.Equal(123, await Page.EvaluateExpressionAsync<int>("window.result"));
        }
    }
}
