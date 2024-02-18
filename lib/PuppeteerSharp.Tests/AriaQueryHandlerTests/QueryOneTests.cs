using System;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using PuppeteerSharp.Nunit;
using static System.Net.Mime.MediaTypeNames;

namespace PuppeteerSharp.Tests.AriaQueryHandlerTests
{
    public class QueryOneTests : PuppeteerPageBaseTest
    {
        public QueryOneTests() : base()
        {
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryOne", "should find button by role")]
        public async Task ShouldFindButtonByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/[role='button']");
            var id = await button.EvaluateFunctionAsync<string>("(button) => button.id");
            Assert.AreEqual("btn", id);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryOne", "should find button by name and role")]
        public async Task ShouldFindButtonNameAndByRole()
        {
            await Page.SetContentAsync("<div id='div'><button id='btn' role='button'>Submit</button></div>");
            var button = await Page.QuerySelectorAsync("aria/Submit[role='button']");
            var id = await button.EvaluateFunctionAsync<string>("(button) => button.id");
            Assert.AreEqual("btn", id);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryOne", "should find first matching element")]
        public async Task ShouldFindFirstMatchingElement()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu div""></div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu div""></div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu div");
            var id = await button.EvaluateFunctionAsync<string>("(button) => button.id");
            Assert.AreEqual("mnu1", id);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryOne", "should find by name")]
        public async Task ShouldFindByName()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu-label1"">menu div</div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu-label2"">menu div</div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu-label1");
            var id = await button.EvaluateFunctionAsync<string>("(button) => button.id");
            Assert.AreEqual("mnu1", id);
        }

        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryOne", "should find by name")]
        public async Task ShouldFindByName2()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu-label1"">menu div</div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu-label2"">menu div</div>
            ");
            var button = await Page.QuerySelectorAsync("aria/menu-label2");
            var id = await button.EvaluateFunctionAsync<string>("(button) => button.id");
            Assert.AreEqual("mnu2", id);
        }
    }
}
