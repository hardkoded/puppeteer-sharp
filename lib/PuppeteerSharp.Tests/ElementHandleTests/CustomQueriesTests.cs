using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class CustomQueriesTests : PuppeteerPageBaseTest
    {
        public CustomQueriesTests() : base()
        {
        }

        [TearDown]
        public void ClearCustomQueryHandlers()
        {
            Browser.ClearCustomQueryHandlers();
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should register and unregister")]
        public async Task ShouldRegisterAndUnregister()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div id='foo'></div>");

            Browser.RegisterCustomQueryHandler("getById", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`[id='${selector}']`)",
            });

            var element = await Page.QuerySelectorAsync("getById/foo");
            Assert.That(await Page.EvaluateFunctionAsync<string>(
                @"(el) => el.id",
                element), Is.EqualTo("foo"));

            var handlerNamesAfterRegistering = ((Browser)Browser).GetCustomQueryHandlerNames();
            Assert.That(handlerNamesAfterRegistering.ToArray(), Does.Contain("getById"));

            // Unregister.
            Browser.UnregisterCustomQueryHandler("getById");
            try
            {
                await Page.QuerySelectorAsync("getById/foo");
                throw new PuppeteerException("Custom query handler name not set - throw expected");
            }
            catch (Exception ex)
            {
                Assert.That(ex.Message, Does.Not.Contain($"Custom query handler name not set - throw expected"));
            }

            var handlerNamesAfterUnregistering = ((Browser)Browser).GetCustomQueryHandlerNames();
            Assert.That(handlerNamesAfterUnregistering, Does.Not.Contain("getById"));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should throw with invalid query names")]
        public void ShouldThrowWithInvalidQueryNames()
        {
            var ex = Assert.Throws<PuppeteerException>(() => Browser.RegisterCustomQueryHandler("1/2/3", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`[id='${selector}']`)",
            }));

            Assert.That(ex.Message, Is.EqualTo("Custom query handler names may only contain [a-zA-Z]"));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should work for multiple elements")]
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

            Assert.That(classNames.ToArray(), Is.EqualTo(new[] { "foo", "foo baz" }));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should eval correctly")]
        public async Task ShouldEvalCorrectly()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div class='foo'></div><div class='foo baz'>Foo2</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var elements = await Page.QuerySelectorAllHandleAsync("getByClass/foo")
                .EvaluateFunctionAsync<int>("(divs) =>  divs.length");

            Assert.That(elements, Is.EqualTo(2));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should wait correctly with waitForSelector")]
        public async Task ShouldWaitCorrectlyWithWaitForSelector()
        {
            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
            });

            var waitFor = Page.WaitForSelectorAsync("getByClass/foo");

            await Page.SetContentAsync("<div id='not-foo'></div><div class='foo'></div>");

            var element = await waitFor;

            Assert.That(element, Is.Not.Null);
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should wait correctly with waitForSelector on an element")]
        public async Task ShouldWaitCorrectlyWithWaitForSelectorOnAnElement()
        {
            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
            });

            var waitFor = Page.WaitForSelectorAsync("getByClass/foo");

            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"bar\">bar2</div><div class=\"foo\">Foo1</div>");

            var element = await waitFor;

            Assert.That(element, Is.Not.Null);

            var innerWaitFor = element.WaitForSelectorAsync("getByClass/bar");

            await element.EvaluateFunctionAsync("(el) => el.innerHTML = '<div class=\"bar\">bar1</div>'");

            element = await innerWaitFor;
            Assert.That(await element.EvaluateFunctionAsync<string>("(el) => el.innerText"), Is.EqualTo("bar1"));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should work when both queryOne and queryAll are registered")]
        public async Task ShouldWorkWhenBothQueryOneAndQueryAllAreRegistered()
        {
            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"foo\"><div id=\"nested-foo\" class=\"foo\"/></div><div class=\"foo baz\">Foo2</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var element = await Page.QuerySelectorAsync("getByClass/foo");
            Assert.That(element, Is.Not.Null);
            var elements = await Page.QuerySelectorAllAsync("getByClass/foo");
            Assert.That(elements, Has.Length.EqualTo(3));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Custom queries", "should eval when both queryOne and queryAll are registered")]
        public async Task ShouldEvalWhenBothQueryOneAndQueryAllAreRegistered()
        {
            await Page.SetContentAsync("<div id=\"not-foo\"></div><div class=\"foo\">text</div><div class=\"foo baz\">content</div>");

            Browser.RegisterCustomQueryHandler("getByClass", new CustomQueryHandler
            {
                QueryOne = "(element, selector) => element.querySelector(`.${selector}`)",
                QueryAll = "(element, selector) => element.querySelectorAll(`.${selector}`)",
            });

            var txtContent = await Page.QuerySelectorAsync("getByClass/foo")
                .EvaluateFunctionAsync<string>("(div) =>  div.textContent");

            Assert.That(txtContent, Is.EqualTo("text"));

            var txtContents = await Page.QuerySelectorAllHandleAsync("getByClass/foo")
                .EvaluateFunctionAsync<string>("(divs) =>  divs.map((d) => d.textContent).join('')");

            Assert.That(txtContents, Is.EqualTo("textcontent"));
        }
    }
}
