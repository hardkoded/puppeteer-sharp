using System;
using System.Threading.Tasks;
using NUnit.Framework;
using PuppeteerSharp.Nunit;

namespace PuppeteerSharp.Tests.PageTests
{
    public class GetContentTests : PuppeteerPageBaseTest
    {

        [Test, PuppeteerTest("puppeteer-sharp.spec.ts", "PuppeteerSharp", "should work with lone surrogate")]
        public async Task ShouldWorkWithLoneSurrogate()
        {
            await Page.GoToAsync(TestConstants.ServerUrl + "/lone-surrogate.html");
            var result = await Page.GetContentAsync(new GetContentOptions { ReplaceLoneSurrogates = true });

            Assert.That(result, Contains.Substring("This paragraph contains a lone surrogate"));
        }
    }
}
