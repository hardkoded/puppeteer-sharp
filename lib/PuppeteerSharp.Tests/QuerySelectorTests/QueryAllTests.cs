using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    public class QueryAllTests : PuppeteerPageBaseTest
    {
        [SetUp]
        public void RegisterCustomQueryHandler()
        {
            Browser.RegisterCustomQueryHandler("allArray", new CustomQueryHandler
            {
                QueryAll = "(element, selector) => Array.from(element.querySelectorAll(selector))"
            });
        }

        [TearDown]
        public void ClearCustomQueryHandlers()
        {
            Browser.ClearCustomQueryHandlers();
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "should have registered handler")]
        public void ShouldHaveRegisteredHandler()
        {
            Assert.That(((Browser)Browser).GetCustomQueryHandlerNames().ToArray(), Does.Contain("allArray"));
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "$$ should query existing elements")]
        public async Task QuerySelectorAllShouldQueryExistingElements()
        {
            await Page.SetContentAsync("<html><body><div>A</div><br/><div>B</div></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("allArray/div");
            Assert.That(elements, Has.Length.EqualTo(2));
            var tasks = elements.Select((element) =>
              Page.EvaluateFunctionAsync<string>("(e) => e.textContent", element)
            );
            Assert.That(await Task.WhenAll(tasks), Is.EqualTo(new[] { "A", "B" }));
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "$$ should return empty array for non-existing elements")]
        public async Task QuerySelectorAllShouldReturnEmptyArrayForNonExistingElements()
        {
            await Page.SetContentAsync("<html><body><span>A</span><br/><span>B</span></body></html>");
            var html = await Page.QuerySelectorAsync("html");
            var elements = await html.QuerySelectorAllAsync("allArray/div");
            Assert.That(elements, Is.Empty);
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "$$eval should work")]
        public async Task QuerySelectorAllEvalShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("allArray/div")
                .EvaluateFunctionAsync<int>("(divs) => divs.length");

            Assert.That(divsCount, Is.EqualTo(3));
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "$$eval should accept extra arguments")]
        public async Task QuerySelectorAllEvalShouldAcceptExtraArguments()
        {
            await Page.SetContentAsync("<div>hello</div><div>beautiful</div><div>world!</div>");
            var divsCount = await Page.QuerySelectorAllHandleAsync("allArray/div")
                .EvaluateFunctionAsync<int>("(divs, two, three) => divs.length + two + three", 2, 3);

            Assert.That(divsCount, Is.EqualTo(8));
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "$$eval should accept ElementHandles as arguments")]
        public async Task ShouldAcceptElementHandlesAsArguments()
        {
            await Page.SetContentAsync("<section>2</section><section>2</section><section>1</section><div>3</div>");
            var divHandle = await Page.QuerySelectorAsync("div");
            var text = await Page.QuerySelectorAllHandleAsync("allArray/section")
                .EvaluateFunctionAsync<int>(@"(sections, div) =>
                    sections.reduce(
                        (acc, section) => acc + Number(section.textContent),
                        0
                    ) + Number(div.textContent)", divHandle);

            Assert.That(text, Is.EqualTo(8));
        }

        [Test, PuppeteerTest("queryselector.spec", "QueryAll", "should handle many elements")]
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
            Assert.That(sum, Is.EqualTo(500500));
        }
    }
}
