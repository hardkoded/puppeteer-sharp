using System.Collections.Generic;
using System.Text;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class AttributeTests : DevToolsContextBaseTest
    {
        public AttributeTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldGetAttribute()
        {
            const string expected = "checkbox";

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");
            var actual = await checkbox.GetAttributeAsync<string>("type");

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetAttribute()
        {
            const int expected = 1676;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await DevToolsContext.QuerySelectorAsync<HtmlInputElement>("#agree");
            await checkbox.SetAttributeAsync("data-custom", expected);

            var actual = await checkbox.GetAttributeAsync<int>("data-custom");

            Assert.Equal(expected, actual);
        }
    }
}
