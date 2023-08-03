using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class QueryAllTests : PuppeteerPageBaseTest
    {
        public QueryAllTests(): base()
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            Browser.RegisterCustomQueryHandler("allArray", new CustomQueryHandler
            {
                QueryAll = "(element, selector) => Array.from(element.querySelectorAll(selector))"
            });
        }
        public override Task DisposeAsync()
        {
            Browser.ClearCustomQueryHandlers();
            return base.DisposeAsync();
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "should have registered handler")]
        [PuppeteerTimeout]
        public void ShouldHaveRegisteredHandler()
        {
            StringAssert.Contains("allArray", ((Browser)Browser).GetCustomQueryHandlerNames());
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "$$ should query existing elements")]
        [PuppeteerTimeout]
        public async Task QuerySelectorAllShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("allArray/div");
            Assert.AreEqual(2, elements.Length);
            var tasks= elements.Select((element) =>
              Page.EvaluateFunctionAsync<string>("(e) => e.textContent", element)
            );
            Assert.AreEqual(new[] { "A", "B"}, await Task.WhenAll(tasks));
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "$$ should return empty array for non-existing elements")]
        [PuppeteerTimeout]
        public async Task QuerySelectorAllShouldReturnEmptyArrayForNonExistingElements()
        {
            await Page.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("allArray/div");
            Assert.IsEmpty(elements);
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "$$eval should work")]
        [PuppeteerTimeout]
        public async Task QuerySelectorAllEvalShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("allArray/div")
                .EvaluateFunctionAsync<int>("(divs) => divs.length");

            Assert.AreEqual(3, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "$$eval should accept extra arguments")]
        [PuppeteerTimeout]
        public async Task QuerySelectorAllEvalShouldAcceptExtraArguments()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("allArray/div")
                .EvaluateFunctionAsync<int>("(divs, two, three) => divs.length + two + three", 2, 3);

            Assert.AreEqual(8, divsCount);
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "$$eval should accept ElementHandles as arguments")]
        [PuppeteerTimeout]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>2</section><section>2</section><section>1</section><div>3</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var text = await Page.QuerySelectorAllHandleAsync("allArray/section")
                .EvaluateFunctionAsync<string>(@"(sections, div) =>
                    sections.reduce(
                        (acc, section) => acc + Number(section.textContent),
                        0
                    ) + Number(div.textContent)", divHandle);
            Assert.AreEqual("8", text);
        }

        [PuppeteerTest("queryselector.spec.ts", "QueryAll", "should handle many elements")]
        [PuppeteerTimeout]
        public async Task ShouldHandleManyElements()
        {
            await Page.EvaluateExpressionAsync(@"
                for (var i = 0; i <= 1000; i++) {
                    const section = document.createElement('section');
                    section.textContent = i;
                    document.body.appendChild(section);
                }
            ");

            var sum = await Page
                .QuerySelectorAllHandleAsync("allArray/section")
                .EvaluateFunctionAsync<int>(@"(sections, div) =>
                sections.reduce((acc, section) => acc + Number(section.textContent), 0)");
            Assert.AreEqual(500500, sum);
        }
    }
}
