using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.QueryHandlerTests.TextSelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TextSelectorInPageTests : PuppeteerPageBaseTest
    {
        public TextSelectorInPageTests(): base()
        {
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            Assert.NotNull(await Page.QuerySelectorAsync("text/test"));
            Assert.Single(await Page.QuerySelectorAllAsync("text/test"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should return empty array for non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            Assert.Null(await Page.QuerySelectorAsync("text/test"));
            Assert.Empty(await Page.QuerySelectorAllAsync("text/test"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should return first element")]
        [PuppeteerFact]
        public async Task ShouldReturnFirstElement()
        {
            await Page.SetContentAsync("<div id=\"1\">a</div><div>a</div>");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.Equal("1", await element.EvaluateFunctionAsync("element => element.id"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should return multiple elements")]
        [PuppeteerFact]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div>a</div><div>a</div>");
            Assert.Equal(2, (await Page.QuerySelectorAllAsync("text/a")).Length);
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should pierce shadow DOM")]
        [PuppeteerFact]
        public async Task ShouldPierceShadowDom()
        {
            await Page.EvaluateFunctionAsync(@"() => {
                const div = document.createElement('div');
                const shadow = div.attachShadow({mode: 'open'});
                const diva = document.createElement('div');
                shadow.append(diva);
                const divb = document.createElement('div');
                shadow.append(divb);
                diva.innerHTML = 'a';
                divb.innerHTML = 'b';
                document.body.append(div);
            }");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.Equal("a", await element.EvaluateFunctionAsync("element => element.textContent"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should query deeply nested text")]
        [PuppeteerFact]
        public async Task ShouldQueryDeeplyNestedText()
        {
            await Page.SetContentAsync("<div><div>a</div><div>b</div></div>");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.Equal("a", await element.EvaluateFunctionAsync("element => element.textContent"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should query inputs")]
        [PuppeteerFact]
        public async Task ShouldQueryInputs()
        {
            await Page.SetContentAsync("<input value=\"a\">");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.Equal("a", await element.EvaluateFunctionAsync("element => element.value"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should not query radio")]
        [PuppeteerFact]
        public async Task ShouldNotQueryRadio()
        {
            await Page.SetContentAsync("<radio value=\"a\">");
            Assert.Null(await Page.QuerySelectorAsync("text/a"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should query text spanning multiple elements")]
        [PuppeteerFact]
        public async Task ShouldQueryTextSpanningMultipleElements()
        {
            await Page.SetContentAsync("<div><span>a</span> <span>b</span><div>");
            var element = await Page.QuerySelectorAsync("text/a b");
            Assert.Equal("a b", await element.EvaluateFunctionAsync("element => element.textContent"));
        }
    }
}
