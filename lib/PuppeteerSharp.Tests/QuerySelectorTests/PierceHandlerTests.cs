using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PierceHandlerTests : PuppeteerPageBaseTest
    {
        public PierceHandlerTests(ITestOutputHelper output) : base(output)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await Page.SetContentAsync(@"
                <script>
                const div = document.createElement('div');
                const shadowRoot = div.attachShadow({mode: 'open'});
                const div1 = document.createElement('div');
                div1.textContent = 'Hello';
                div1.className = 'foo';
                const div2 = document.createElement('div');
                div2.textContent = 'World';
                div2.className = 'foo';
                shadowRoot.appendChild(div1);
                shadowRoot.appendChild(div2);
                document.documentElement.appendChild(div);
                </script>
                ");
        }
        public override Task DisposeAsync()
        {
            Browser.ClearCustomQueryHandlers();
            return base.DisposeAsync();
        }

        [PuppeteerTest("queryselector.spec.ts", "pierceHandler", "should find first element in shadow")]
        [PuppeteerFact]
        public async Task ShouldFindFirstElementInShadow()
        {
            var div = await Page.QuerySelectorAsync("pierce/.foo");
            var text = await div.EvaluateFunctionAsync<string>(@"(element) => {
                return element.textContent;
            }");
            Assert.Equal("Hello", text);
        }

        [PuppeteerTest("queryselector.spec.ts", "pierceHandler", "should find all elements in shadow")]
        [PuppeteerFact]
        public async Task ShouldFindAllElementsInShadow()
        {
            var divs = await Page.QuerySelectorAllAsync("pierce/.foo");
            var text = await Task.WhenAll(
                divs.Select(div => {
                    return div.EvaluateFunctionAsync<string>(@"(element) => {
                        return element.textContent;
                    }");
                }));
            Assert.Equal("Hello World", string.Join(" ", text));
        }

        [PuppeteerTest("queryselector.spec.ts", "pierceHandler", "should find first child element")]
        [PuppeteerFact]
        public async Task ShouldFindFirstChildElement()
        {
            var parentElement = await Page.QuerySelectorAsync("html > div");
            var childElement = await parentElement.QuerySelectorAsync("pierce/div");
            var text = await childElement.EvaluateFunctionAsync<string>(@"(element) => {
                return element.textContent;
            }");
            Assert.Equal("Hello", text);
        }

        [PuppeteerTest("queryselector.spec.ts", "pierceHandler", "should find all child elements")]
        [PuppeteerFact]
        public async Task ShouldFindAllChildElements()
        {
            var parentElement = await Page.QuerySelectorAsync("html > div");
            var childElements = await parentElement.QuerySelectorAllAsync("pierce/div");
            var text = await Task.WhenAll(
                childElements.Select(div => {
                    return div.EvaluateFunctionAsync<string>(@"(element) => {
                        return element.textContent;
                    }");
                }));
            Assert.Equal("Hello World", string.Join(" ", text));
        }
    }
}
