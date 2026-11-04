using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    public class WaitForSelectorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Element.waitForSelector", "should wait correctly with waitForSelector on an element")]
        public async Task ShouldWaitCorrectlyWithWaitForSelectorOnAnElement()
        {
            var waitFor = Page.WaitForSelectorAsync(".foo");

            // Set the page content after the waitFor has been started.
            await Page.SetContentAsync(
                "<div id=\"not-foo\"></div><div class=\"bar\">bar2</div><div class=\"foo\">Foo1</div>");

            var element = await waitFor;
            Assert.That(element, Is.Not.Null);

            var innerWaitFor = element.WaitForSelectorAsync(".bar");

            await element.EvaluateFunctionAsync("(el) => el.innerHTML = '<div class=\"bar\">bar1</div>'");

            var element2 = await innerWaitFor;
            Assert.That(element2, Is.Not.Null);
            Assert.That(
                await element2.EvaluateFunctionAsync<string>("(el) => el.innerText"),
                Is.EqualTo("bar1"));
        }

        [Test, PuppeteerTest("elementhandle.spec", "ElementHandle specs Element.waitForSelector", "should wait correctly with waitForSelector and xpath on an element")]
        public async Task ShouldWaitCorrectlyWithWaitForSelectorAndXPathOnAnElement()
        {
            // Set the page content after the waitFor has been started.
            await Page.SetContentAsync(
                "<div id=\"el1\">el1<div id=\"el2\">el2</div></div><div id=\"el3\">el3</div>");

            var elById = await Page.WaitForSelectorAsync("#el1");

            var elByXpath = await elById.WaitForSelectorAsync("xpath/.//div");
            Assert.That(
                await elByXpath.EvaluateFunctionAsync<string>("(el) => el.id"),
                Is.EqualTo("el2"));
        }
    }
}
