using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.QueryHandlerTests
{
    public class PSelectorTests : PuppeteerPageBaseTest
    {
        public PSelectorTests() : base()
        {
        }

        [TearDown]
        public void ClearCustomQueryHandlers()
        {
            Browser.ClearCustomQueryHandlers();
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with CSS selectors")]
        public async Task ShouldWorkWithCssSelectors()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("div > button");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with puppeteer pseudo classes")]
        public async Task ShouldWorkWithPuppeteerPseudoClasses()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("button::-p-text(world)");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with deep combinators")]
        public async Task ShouldWorkWithDeepCombinators()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");

            // div >>>> div (direct shadow child combinator)
            var element = await Page.QuerySelectorAsync("div >>>> div");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("c"));

            // div >>> div (deep descendant combinator)
            var elements = await Page.QuerySelectorAllAsync("div >>> div");
            Assert.That(await elements.ElementAt(1).EvaluateFunctionAsync<string>("element => element.id"), Is.EqualTo("d"));

            // #c >>>> div
            var elements2 = await Page.QuerySelectorAllAsync("#c >>>> div");
            Assert.That(await elements2.First().EvaluateFunctionAsync<string>("element => element.id"), Is.EqualTo("d"));

            // #c >>> div
            var elements3 = await Page.QuerySelectorAllAsync("#c >>> div");
            Assert.That(await elements3.First().EvaluateFunctionAsync<string>("element => element.id"), Is.EqualTo("d"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with text selectors")]
        public async Task ShouldWorkWithTextSelectors()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("div ::-p-text(world)");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work ARIA selectors")]
        public async Task ShouldWorkAriaSelectors()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("div ::-p-aria(world)");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work for ARIA selectors in multiple isolated worlds")]
        public async Task ShouldWorkForAriaSelectorsInMultipleIsolatedWorlds()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.WaitForSelectorAsync("::-p-aria(world)");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));

            // $ would add ARIA query handler to the main world.
            await element.QuerySelectorAsync("::-p-aria(world)");

            var element2 = await Page.WaitForSelectorAsync("::-p-aria(world)");
            Assert.That(element2, Is.Not.Null);
            var id2 = await element2.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id2, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work ARIA selectors with role")]
        public async Task ShouldWorkAriaSelectorsWithRole()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("::-p-aria(world[role=\"button\"])");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work ARIA selectors with name and role")]
        public async Task ShouldWorkAriaSelectorsWithNameAndRole()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("::-p-aria([name=\"world\"][role=\"button\"])");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work XPath selectors")]
        public async Task ShouldWorkXPathSelectors()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("div ::-p-xpath(//button)");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("b"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with custom selectors")]
        public async Task ShouldWorkWithCustomSelectors()
        {
            Browser.RegisterCustomQueryHandler("div", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => document.querySelector('div')",
            });

            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync("::-p-div");
            Assert.That(element, Is.Not.Null);
            var id = await element.EvaluateFunctionAsync<string>("element => element.id");
            Assert.That(id, Is.EqualTo("a"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with custom selectors with args")]
        public async Task ShouldWorkWithCustomSelectorsWithArgs()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            Browser.RegisterCustomQueryHandler("div", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => selector === 'true' ? document.querySelector('div') : document.querySelector('button')",
            });

            {
                var element = await Page.QuerySelectorAsync("::-p-div(true)");
                Assert.That(element, Is.Not.Null);
                var id = await element.EvaluateFunctionAsync<string>("element => element.id");
                Assert.That(id, Is.EqualTo("a"));
            }
            {
                var element = await Page.QuerySelectorAsync("::-p-div(\"true\")");
                Assert.That(element, Is.Not.Null);
                var id = await element.EvaluateFunctionAsync<string>("element => element.id");
                Assert.That(id, Is.EqualTo("a"));
            }
            {
                var element = await Page.QuerySelectorAsync("::-p-div('true')");
                Assert.That(element, Is.Not.Null);
                var id = await element.EvaluateFunctionAsync<string>("element => element.id");
                Assert.That(id, Is.EqualTo("a"));
            }
            {
                var element = await Page.QuerySelectorAsync("::-p-div");
                Assert.That(element, Is.Not.Null);
                var id = await element.EvaluateFunctionAsync<string>("element => element.id");
                Assert.That(id, Is.EqualTo("b"));
            }
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with :hover")]
        public async Task ShouldWorkWithHover()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var button = await Page.QuerySelectorAsync("div ::-p-text(world)");
            Assert.That(button, Is.Not.Null);
            await button.HoverAsync();

            var button2 = await Page.QuerySelectorAsync("div ::-p-text(world):hover");
            Assert.That(button2, Is.Not.Null);
            var textContent = await button2.EvaluateFunctionAsync<string>("span => span.textContent");
            Assert.That(textContent, Is.EqualTo("world"));
            var tagName = await button2.EvaluateFunctionAsync<string>("span => span.tagName");
            Assert.That(tagName, Is.EqualTo("BUTTON"));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should match querySelector* ordering")]
        public async Task ShouldMatchQuerySelectorOrdering()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            foreach (var list in Permute(new[] { "div", "button", "span" }))
            {
                var selector = string.Join(",", list.Select(s => s == "button" ? "::-p-text(world)" : s));
                var elements = await Page.QuerySelectorAllAsync(selector);
                var actual = new List<string>();
                foreach (var element in elements)
                {
                    actual.Add(await element.EvaluateFunctionAsync<string>("element => element.id"));
                }

                Assert.That(string.Join(",", actual), Is.EqualTo("a,b,f,c"));
            }
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should work with selector lists")]
        public async Task ShouldWorkWithSelectorLists()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var elements = await Page.QuerySelectorAllAsync("div, ::-p-text(world)");
            Assert.That(elements.Count(), Is.EqualTo(3));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should not have duplicate elements from selector lists")]
        public async Task ShouldNotHaveDuplicateElementsFromSelectorLists()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var elements = await Page.QuerySelectorAllAsync("::-p-text(world), button");
            Assert.That(elements.Count(), Is.EqualTo(1));
        }

        [Test, PuppeteerTest("queryhandler.spec", "Query handler tests P selectors", "should handle escapes")]
        public async Task ShouldHandleEscapes()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/p-selectors.html");
            var element = await Page.QuerySelectorAsync(
                ":scope >>> ::-p-text(My name is Jun \\(pronounced like \"June\"\\))");
            Assert.That(element, Is.Not.Null);

            var element2 = await Page.QuerySelectorAsync(
                ":scope >>> ::-p-text(\"My name is Jun (pronounced like \\\"June\\\")\")");
            Assert.That(element2, Is.Not.Null);

            var element3 = await Page.QuerySelectorAsync(
                ":scope >>> ::-p-text(My name is Jun \\(pronounced like \"June\"\\)\")");
            Assert.That(element3, Is.Null);

            var element4 = await Page.QuerySelectorAsync(
                ":scope >>> ::-p-text(\"My name is Jun \\(pronounced like \"June\"\\))");
            Assert.That(element4, Is.Null);
        }

        private static List<T[]> Permute<T>(T[] inputs)
        {
            var results = new List<T[]>();
            for (var i = 0; i < inputs.Length; i++)
            {
                var remaining = inputs.Take(i).Concat(inputs.Skip(i + 1)).ToArray();
                var permutations = Permute(remaining);
                var value = inputs[i];
                if (permutations.Count == 0)
                {
                    results.Add(new[] { value });
                    continue;
                }

                foreach (var part in permutations)
                {
                    results.Add(new[] { value }.Concat(part).ToArray());
                }
            }

            return results;
        }
    }
}
