using System;
using System.Collections.Generic;
using System.Text;
using CefSharp.DevTools.Dom;
using PuppeteerSharp.Tests.Attributes;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class InvokeMemberTests : DevToolsContextBaseTest
    {
        public InvokeMemberTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerFact]
        public async Task ShouldWork()
        {
            await DevToolsContext.GoToAsync(TestConstants.ServerUrl + "/input/checkbox.html");
            var checkbox = await DevToolsContext.QuerySelectorAsync("#agree");
            await checkbox.InvokeMemberAsync("click");

            var actual = await checkbox.GetPropertyValueAsync<bool>("checked");

            Assert.True(actual);
        }
    }
}
