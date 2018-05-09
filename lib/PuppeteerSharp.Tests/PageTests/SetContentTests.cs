﻿using System.Threading.Tasks;
using Xunit;

namespace PuppeteerSharp.Tests.PageTests
{
    [Collection("PuppeteerLoaderFixture collection")]
    public class SetContentTests : PuppeteerPageBaseTest
    {
        const string ExpectedOutput = "<html><head></head><body><div>hello</div></body></html>";

        [Fact]
        public async Task ShouldWork()
        {
            await Page.SetContentAsync("<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal(ExpectedOutput, result);
        }

        [Fact]
        public async Task ShouldWorkWithDoctype()
        {
            const string doctype = "<!DOCTYPE html>";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }

        [Fact]
        public async Task ShouldWorkWithHtml4Doctype()
        {
            const string doctype = "<!DOCTYPE html PUBLIC \" -//W3C//DTD HTML 4.01//EN\" " +
                "\"http://www.w3.org/TR/html4/strict.dtd\">";

            await Page.SetContentAsync($"{doctype}<div>hello</div>");
            var result = await Page.GetContentAsync();

            Assert.Equal($"{doctype}{ExpectedOutput}", result);
        }
    }
}
