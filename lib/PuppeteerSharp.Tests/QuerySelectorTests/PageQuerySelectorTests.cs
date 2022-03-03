using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using CefSharp;
using CefSharp.Puppeteer;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class PageQuerySelectorTests : PuppeteerPageBaseTest
    {
        public PageQuerySelectorTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
        async Task Usage(IWebBrowser chromiumWebBrowser)
#pragma warning restore IDE0051 // Remove unused private members
        {
            #region QuerySelector
            // Wait for Initial page load
            await chromiumWebBrowser.WaitForInitialLoadAsync();

            await using var devtoolsContext = await chromiumWebBrowser.GetDevToolsContextAsync();

            var element = await devtoolsContext.QuerySelectorAsync("#myElementId");

            // Get a custom attribute value
            var customAttribute = await element.GetAttributeValueAsync<string>("data-customAttribute");

            //Set innerText property for the element
            await element.SetPropertyValueAsync("innerText", "Welcome!");

            //Get innerText property for the element
            var innerText = await element.GetPropertyValueAsync<string>("innerText");

            //Get all child elements
            var childElements = await element.QuerySelectorAllAsync("div");

            //Click The element
            await element.ClickAsync();

            var divElements = await devtoolsContext.QuerySelectorAllAsync("div");

            foreach(var div in divElements)
            {
                var style = await div.GetAttributeValueAsync<string>("style");
                await div.SetAttributeValueAsync("data-customAttribute", "123");
                await div.SetPropertyValueAsync("innerText", "Updated Div innerText");
            }

            #endregion
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldQueryExistingElement()
        {
            await DevToolsContext.SetContentAsync("<section>test</section>");
            var element = await DevToolsContext.QuerySelectorAsync("section");
            Assert.NotNull(element);
        }

        [PuppeteerTest("queryselector.spec.ts", "Page.$", "should query existing element")]
        [PuppeteerFact]
        public async Task ShouldReturnNullForNonExistingElement()
        {
            var element = await DevToolsContext.QuerySelectorAsync("non-existing-element");
            Assert.Null(element);
        }
    }
}
