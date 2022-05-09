using System;
using System.Linq;
using System.Threading.Tasks;
using PuppeteerSharp.Tests.Attributes;
using PuppeteerSharp.Xunit;
using Xunit;
using Xunit.Abstractions;

namespace PuppeteerSharp.Tests.ElementHandleTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class CustomQueriesTests : PuppeteerPageBaseTest
    {
        public CustomQueriesTests(ITestOutputHelper output) : base(output)
        {
        }

        [PuppeteerTest("elementhandle.spec.ts", "Custom queries", "should reguster and unregister")]
        [PuppeteerFact]
        public async Task ShouldRegisterAndUnregister()
        {
            await Page.SetContentAsync("<div id='not-foo'></div><div id='foo'></div>");

            Puppeteer.RegisterCustomQueryHandler("getById", new CustomQueryHandler
            { 
                QueryOne = "(element, selector) => document.querySelector(`[id='${ selector}']`)",
            });

            var element = await Page.QuerySelectorAsync("getById/foo");
            Assert.Equal("foo", await Page.EvaluateFunctionAsync<string>(
                @"(el) => el.id",
                element));

            var handlerNamesAfterRegistering = Puppeteer.GetCustomQueryHandlerNames();
            Assert.Contains("getById", handlerNamesAfterRegistering);

            // Unregister.
            Puppeteer.UnregisterCustomQueryHandler("getById");
            try
            {
                await Page.QuerySelectorAsync("getById/foo");
                throw new PuppeteerException("Custom query handler name not set - throw expected");
            }
            catch (Exception ex)
            {
                Assert.Equal($"Query set to use \"getById\", but no query handler of that name was found", ex.Message);
            }

            var handlerNamesAfterUnregistering = Puppeteer.GetCustomQueryHandlerNames();
            Assert.DoesNotContain("getById", handlerNamesAfterUnregistering);
        }
    }
}
