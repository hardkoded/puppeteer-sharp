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
    public class QueryAllTests : PuppeteerPageBaseTest
    {
        [Test, Retry(2), PuppeteerTest("ariaqueryhandler.spec", "queryAll", "should find menu by name")]
        public async Task ShouldFindMenuByName()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu div""></div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu div""></div>
            ");
            var divs = await Page.QuerySelectorAllAsync("aria/menu div");
            var ids = await Task.WhenAll(divs.Select(div => div.EvaluateFunctionAsync<string>("div => div.id")));

            Assert.AreEqual("mnu1, mnu2", String.Join(", ", ids));
        }
    }
}
