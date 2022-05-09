using System;
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

            var handlerNamesAfterRegistering = Puppeteer.CustomQueryHandlerNames();
            Assert.True(handlerNamesAfterRegistering.Contains("getById"));

            // Unregister.
            Puppeteer.UnregisterCustomQueryHandler("getById");
            try
            {
                await page.,('getById/foo');
                throw new Error('Custom query handler name not set - throw expected');
            }
            catch (error)
            {
                expect(error).toStrictEqual(
                  new Error(
                    'Query set to use "getById", but no query handler of that name was found'
                  )
                );
            }
            const handlerNamesAfterUnregistering =
              puppeteer.customQueryHandlerNames();
            expect(handlerNamesAfterUnregistering.includes('getById')).toBeFalsy();
        }
    }
}
