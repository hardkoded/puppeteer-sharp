using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class ParseAriaSelectorTests : PuppeteerPageBaseTest
    {
        public ParseAriaSelectorTests(): base()
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            await Page.SetContentAsync(@"
                <button id=""btn"" role=""button""> Submit  button   and some spaces  </button>
            ");
        }
        public override Task DisposeAsync()
        {
            return base.DisposeAsync();
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "parseAriaSelector", "should find button")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindButton()
        {
            async Task ExpectFound(IElementHandle button)
            {
                Assert.NotNull(button);
                var id = await button.EvaluateFunctionAsync<string>(@"(button) => {
                    return button.id;
                }");
                Assert.Equal("btn", id);
            }

            var button= await Page.QuerySelectorAsync("aria/Submit button and some spaces[role=\"button\"]");
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
            var ex = await Assert.ThrowsAnyAsync<PuppeteerException>(() => Page.QuerySelectorAsync("aria/smth[smth=\"true\"]"));
            Assert.Equal("Unknown aria attribute \"smth\" in selector", ex.Message);
        }
    }
}
