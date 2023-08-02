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
    public class QueryAllTests : PuppeteerPageBaseTest
    {
        public QueryAllTests(): base()
        {
        }

        [PuppeteerTest("ariaqueryhandler.spec.ts", "queryAll", "should find menu by name")]
        [SkipBrowserFact(skipFirefox: true)]
        public async Task ShouldFindMenuByName()
        {
            await Page.SetContentAsync(@"
                <div role=""menu"" id=""mnu1"" aria-label=""menu div""></div>
                <div role=""menu"" id=""mnu2"" aria-label=""menu div""></div>
            ");
            var divs = await Page.QuerySelectorAllAsync("aria/menu div");
            var ids = await Task.WhenAll(divs.Select(div => div.EvaluateFunctionAsync<string>("div => div.id")));

            Assert.Equal("mnu1, mnu2", String.Join(", ", ids));
        }
    }
}
