﻿using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.QueryHandlerTests.TextSelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class TextSelectorInElementHandlesTests : PuppeteerPageBaseTest
    {
        public TextSelectorInElementHandlesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("queryhandler.spec.ts", "in ElementHandles", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\"><span>a</span></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.NotNull(await elementHandle.QuerySelectorAsync("text/a"));
            Assert.Single(await elementHandle.QuerySelectorAllAsync("text/a"));
        }

        [PuppeteerTest("queryhandler.spec.ts", "in Page", "should return null for non-existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            await Page.SetContentAsync("<div class=\"a\"></div>");
            var elementHandle = await Page.QuerySelectorAsync("div");
            Assert.Null(await elementHandle.QuerySelectorAsync("text/a"));
            Assert.Empty(await elementHandle.QuerySelectorAllAsync("text/a"));
        }
    }
}
