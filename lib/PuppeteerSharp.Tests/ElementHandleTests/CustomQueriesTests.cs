using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CustomQueriesTests : PuppeteerPageBaseTest
    {
        public CustomQueriesTests(ITestOutputHelper output) : base(output)
        {
        }

        public override Task DisposeAsync()
        {
            Browser.ClearCustomQueryHandlers();
            return base.DisposeAsync();
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should reguster and unregister")]
        [PuppeteerFact]
        public async Task ShouldRegisterAndUnregister()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div id='foo'></div>");

            Browser.RegisterCustomQueryHandler("getById", new CustomQueryHandler
            { 
                QueryOne = "(element, selector) => element.querySelector(`[id='${selector}']`)",
            });

            var element = await Page.QuerySelectorAsync("getById/foo");
            Assert.Equal("foo", await Page.EvaluateFunctionAsync<string>(
                @"(el) => el.id",
                element));

            var handlerNamesAfterRegistering = Browser.GetCustomQueryHandlerNames();
            Assert.Contains("getById", handlerNamesAfterRegistering);

            // Unregister.
            Browser.UnregisterCustomQueryHandler("getById");
            try
            {
                await Page.QuerySelectorAsync("getById/foo");
                throw new PuppeteerException("Custom query handler name not set - throw expected");
            }
            catch (Exception ex)
            {
                Assert.Equal($"Query set to use \"getById\", but no query handler of that name was found", ex.Message);
            }

            var handlerNamesAfterUnregistering = Browser.GetCustomQueryHandlerNames();
            Assert.DoesNotContain("getById", handlerNamesAfterUnregistering);
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should throw with invalid query names")]
        [PuppeteerFact]
        public void ShouldThrowWithInvalidQueryNames()
        {
            var ex = Assert.Throws<PuppeteerException>(()=> Browser.RegisterCustomQueryHandler("1/2/3", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`[id='${selector}']`)",
            }));

            Assert.Equal("Custom query handler names may only contain [a-zA-Z]", ex.Message);
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should work for multiple elements")]
        [PuppeteerFact]
        public async Task ShouldWorkForMultipleElements()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div class='foo'></div><div class='foo baz'>Foo2</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var elements = await Page.QuerySelectorAllAsync("getByClass/foo");
            var classNames = await Task.WhenAll(
                elements
                    .Select(el => el.EvaluateFunctionAsync<string>("(element) => element.className")));

            Assert.Equal(new[] { "foo", "foo baz" }, classNames.ToArray());
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should eval correctly")]
        [PuppeteerFact]
        public async Task ShouldEvalCorrectly()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div class='foo'></div><div class='foo baz'>Foo2</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var elements = await Page.QuerySelectorAllHandleAsync("getByClass/foo")
                .EvaluateFunctionAsync<int>("(divs) =>  divs.length");
            
            Assert.Equal(2, elements);
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should wait correctly with waitForSelector")]
        [PuppeteerFact]
        public async Task ShouldWaitCorrectlyWithWaitForSelector()
        {
            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
            });

            var waitFor = Page.WaitForSelectorAsync("getByClass/foo");

            await Page.SetContentAsync("<div id='not-foo'></div><div class='foo'></div>");

            var element = await waitFor;

            Assert.NotNull(element);
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should wait correctly with waitForSelector on an element")]
        [PuppeteerFact]
        public async Task ShouldWaitCorrectlyWithWaitForSelectorOnAnElement()
        {
            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
            });

            var waitFor = Page.WaitForSelectorAsync("getByClass/foo");

            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"bar\">bar2</div><div class=\"foo\">Foo1</div>");

            var element = await waitFor;

            Assert.NotNull(element);

            var innerWaitFor = element.WaitForSelectorAsync("getByClass/bar");

            await element.EvaluateFunctionAsync("(el) => el.innerHTML = '<div class=\"bar\">bar1</div>'");

            element = await innerWaitFor;
            Assert.Equal("bar1", await element.EvaluateFunctionAsync<string>("(el) => el.innerText"));
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should work when both queryOne and queryAll are registered")]
        [PuppeteerFact]
        public async Task ShouldWorkWhenBothQueryOneAndQueryAllAreRegistered()
        {
            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"foo\"><div id=\"nested-foo\" class=\"foo\"/></div><div class=\"foo baz\">Foo2</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne= "(element, selector) => element.querySelector(`.${selector}`)",
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var element = await Page.QuerySelectorAsync("getByClass/foo");
            Assert.NotNull(element);
            var elements = await Page.QuerySelectorAllAsync("getByClass/foo");
            Assert.Equal(3, elements.Length);
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should eval when both queryOne and queryAll are registered")]
        [PuppeteerFact]
        public async Task ShouldEvalWhenBothQueryOneAndQueryAllAreRegistered()
        {
            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"foo\">text</div><div class=\"foo baz\">content</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne= "(element, selector) => element.querySelector(`.${selector}`)",
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var txtContent = await Page.QuerySelectorAsync("getByClass/foo")
                .EvaluateFunctionAsync<string>("(div) =>  div.textContent");

            Assert.Equal("text", txtContent);

            var txtContents = await Page.QuerySelectorAllHandleAsync("getByClass/foo")
                .EvaluateFunctionAsync<string>("(divs) =>  divs.map((d) => d.textContent).join('')");

            Assert.Equal("textcontent", txtContents);
        }
    }
}
