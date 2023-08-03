using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Nunit;
using static System.Net.Mime.MediaTypeNames;
using NUnit.Framework;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class QueryOneTests : PuppeteerPageBaseTest
    {
        public QueryOneTests(): base()
        {
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find button by role")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFindButtonByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/[role='button']");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("btn", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find button by name and role")]
        [Skip(SkipAttribute.Targets.Firefox)]
        public async Task ShouldFindButtonNameAndByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/Submit[role='button']");
            var id = await button.EvaluateFunctionAsync("(button) => button.id");
            Assert.Equal("btn", id);
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryOne", "should find first matching element")]
        [Skip(SkipAttribute.Targets.Firefox)]
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
        [Skip(SkipAttribute.Targets.Firefox)]
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
        [Skip(SkipAttribute.Targets.Firefox)]
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
