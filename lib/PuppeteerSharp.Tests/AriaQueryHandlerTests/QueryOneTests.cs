using System;
using System.Linq;
using System.Reflection.PortableExecutable;
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
    public class QueryOneTests : PuppeteerPageBaseTest
    {
        public QueryOneTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find button by role")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindButtonByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/[role='button']");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("btn", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find button by name and role")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindButtonNameAndByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/Submit[role='button']");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("btn", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find first matching element")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindFirstMatchingElement()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu div""></div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu div""></div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu div");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("mnu1", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find by name")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindByName()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu-label1"">menu div</div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu-label2"">menu div</div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu-label1");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("mnu1", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find by name")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindByName2()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu-label1"">menu div</div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu-label2"">menu div</div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu-label2");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("mnu2", id);
        }
    }
}
