using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using PuppeteerSharp.Tests.Attributes;

namespace PuppeteerSharp.Tests.QueryHandlerTests.TextSelectorTests
{
    public class TextSelectorInPageTests : PuppeteerPageBaseTest
    {
        public TextSelectorInPageTests() : base()
        {
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should query existing element")]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<section>test</section>");
            Assert.NotNull(await Page.QuerySelectorAsync("text/test"));
            Assert.That(await Page.QuerySelectorAllAsync("text/test"), Has.Exactly(1).Items);
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should return empty array for non-existing element")]
        public async Task ShouldReturnEmptyArrayForNonExistingElement()
        {
            Assert.Null(await Page.QuerySelectorAsync("text/test"));
            Assert.IsEmpty(await Page.QuerySelectorAllAsync("text/test"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should return first element")]
        public async Task ShouldReturnFirstElement()
        {
            await Page.SetContentAsync("<div id=\"1\">a</div><div>a</div>");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.AreEqual("1", await element.EvaluateFunctionAsync<string>("element => element.id"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should return multiple elements")]
        public async Task ShouldReturnMultipleElements()
        {
            await Page.SetContentAsync("<div>a</div><div>a</div>");
            Assert.AreEqual(2, (await Page.QuerySelectorAllAsync("text/a")).Length);
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should pierce shadow DOM")]
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
            Assert.AreEqual("a", await element.EvaluateFunctionAsync<string>("element => element.textContent"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should query deeply nested text")]
        public async Task ShouldQueryDeeplyNestedText()
        {
            await Page.SetContentAsync("<div><div>a</div><div>b</div></div>");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.AreEqual("a", await element.EvaluateFunctionAsync<string>("element => element.textContent"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should query inputs")]
        public async Task ShouldQueryInputs()
        {
            await Page.SetContentAsync("<input value=\"a\">");
            var element = await Page.QuerySelectorAsync("text/a");
            Assert.AreEqual("a", await element.EvaluateFunctionAsync<string>("element => element.value"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should not query radio")]
        public async Task ShouldNotQueryRadio()
        {
            await Page.SetContentAsync("<radio value=\"a\">");
            Assert.Null(await Page.QuerySelectorAsync("text/a"));
        }

        [Test, PuppeteerTimeout, PuppeteerTest("queryhandler.spec", "in Page", "should query text spanning multiple elements")]
        public async Task ShouldQueryTextSpanningMultipleElements()
        {
            await Page.SetContentAsync("<div><span>a</span> <span>b</span><div>");
            var element = await Page.QuerySelectorAsync("text/a b");
            Assert.AreEqual("a b", await element.EvaluateFunctionAsync<string>("element => element.textContent"));
        }
    }
}
