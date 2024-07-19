using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class ParseAriaSelectorTests : PuppeteerPageBaseTest
    {
        public ParseAriaSelectorTests() : base()
        {
        }

        [SetUp]
        public async Task SetDefaultContentAsync()
        {
            await Page.SetContentAsync(@"
                <button id=""btn"" role=""button""> Submit  button   and some spaces  </button>
            ");
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "parseAriaSelector", "should find button")]
        public async Task ShouldFindButton()
        {
            async Task ExpectFound(IElementHandle button)
            {
                Assert.That(button, Is.Not.Null);
                var id = await button.EvaluateFunctionAsync<string>(@"(button) => {
                    return button.id;
                }");
                Assert.That(id, Is.EqualTo("btn"));
            }

            var button = await Page.QuerySelectorAsync("aria/Submit button and some spaces[role=\"button\"]");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces[role='button']");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/  Submit button and some spaces[role=\"button\"]");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button and some spaces  [role=\"button\"]");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit  button   and  some  spaces   [  role  =  \"button\" ] ");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/[role=\"button\"]Submit button and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/Submit button [role=\"button\"]and some spaces");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/[name=\"  Submit  button and some  spaces\"][role=\"button\"]");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/[name='  Submit  button and some  spaces'][role='button']");
            await ExpectFound(button);
            button = await Page.QuerySelectorAsync("aria/ignored[name=\"Submit  button and some  spaces\"][role=\"button\"]");
            await ExpectFound(button);
            var ex = Assert.ThrowsAsync<PuppeteerException>(() => Page.QuerySelectorAsync("aria/smth[smth=\"true\"]"));
            Assert.That(ex.Message, Is.EqualTo("Unknown aria attribute \"smth\" in selector"));
        }
    }
}
