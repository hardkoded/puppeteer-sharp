using System;
using System.Collections.Generic;
using System.Text;
using CefSharp.Puppeteer;
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
            var checkbox = await DevToolsContext.QuerySelectorAsync("#agree");
            var actual = await checkbox.GetAttributeValueAsync<string>("type");

            Assert.Equal(expected, actual);
        }

        [PuppeteerFact]
        public async Task ShouldSetAttribute()
        {
            const int expected = 1676;

            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await DevToolsContext.QuerySelectorAsync("#agree");
            await checkbox.SetAttributeValueAsync("data-custom", expected);

            var actual = await checkbox.GetAttributeValueAsync<int>("data-custom");

            Assert.Equal(expected, actual);
        }
    }
}
