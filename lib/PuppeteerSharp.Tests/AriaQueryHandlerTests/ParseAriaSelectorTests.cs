using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class ParseAriaSelectorTests : PuppeteerPageBaseTest
    {
        [Test, PuppeteerTest("ariaqueryhandler.spec", "parseAriaSelector", "should handle non-breaking spaces")]
        public async Task ShouldHandleNonBreakingSpaces()
        {
            await Page.SetContentAsync(
                """<button id="btn" role="button"><span>&nbsp;</span><span>&nbsp;</span>Submit button and some spaces</button>"""
            );

            var button = await Page.QuerySelectorAsync("aria/\u00A0\u00A0Submit button and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces");
            Assert.That(button, Is.Null);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "parseAriaSelector", "should handle non-breaking spaces")]
        public async Task ShouldHandleNonBreakingSpaces2()
        {
            await Page.SetContentAsync(
                "<button id=\"btn\" role=\"button\">  Submit button and some spaces</button>"
            );

            var button = await Page.QuerySelectorAsync("aria/ubmit button and some spaces");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces");
            await ExpectFound(button);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "parseAriaSelector", "should handle zero width spaces")]
        public async Task ShouldHandleZeroWidthSpaces()
        {
            await Page.SetContentAsync(
                "<button id=\"btn\" role=\"button\"><span>&ZeroWidthSpace;</span><span>&ZeroWidthSpace;</span>Submit button and some spaces</button>"
            );

            var button = await Page.QuerySelectorAsync("aria/\u200B\u200BSubmit button and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces");
            Assert.That(button, Is.Null);
        }

        [Test, PuppeteerTest("ariaqueryhandler.spec", "parseAriaSelector", "should find button")]
        public async Task ShouldFindButton()
        {
            await Page.SetContentAsync(@"
                <button id=""btn"" role=""button""> Submit  button   and some spaces  </button>
            ");

            var button = await Page.QuerySelectorAsync("aria/Submit button and some spaces[role=\"button\"]");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces[role='button']");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/  Submit button and some spaces[role=\"button\"]");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces  [role=\"button\"]");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/Submit  button   and  some  spaces   [  role  =  \"button\" ] ");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/[role=\"button\"]Submit button and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button [role=\"button\"]and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/[name=\"  Submit  button and some  spaces\"][role=\"button\"]");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/[name='  Submit  button and some  spaces'][role='button']");
            Assert.That(button, Is.Null);
            button = await Page.QuerySelectorAsync("aria/ignored[name=\"Submit button and some spaces\"][role=\"button\"]");
            await ExpectFound(button);
            var ex = Assert.ThrowsAsync<PuppeteerException>(() => Page.QuerySelectorAsync("aria/smth[smth=\"true\"]"));
            Assert.That(ex!.Message, Is.EqualTo("Unknown aria attribute \"smth\" in selector"));
        }

        async Task ExpectFound(IElementHandle handle)
        {
            Assert.That(handle, Is.Not.Null);
            var id = await handle.EvaluateFunctionAsync<string>(@"(button) => {
                    return button.id;
                }");
            Assert.That(id, Is.EqualTo("btn"));
        }
    }
}
