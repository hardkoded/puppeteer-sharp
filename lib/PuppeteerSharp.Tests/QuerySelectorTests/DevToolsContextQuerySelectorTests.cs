using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using PuppeteerSharp.Xunit;
using PuppeteerSharp.Tests.Attributes;
using CefSharp;
using CefSharp.Dom;

namespace PuppeteerSharp.Tests.QuerySelectorTests
{
    [Collection(TestConstants.TestFixtureCollectionName)]
    public class DevToolsContextQuerySelectorTests : DevToolsContextBaseTest
    {
        public DevToolsContextQuerySelectorTests(ITestOutputHelper output) : base(output)
        {
        }

#pragma warning disable xUnit1013 // Public method should be marked as test
        public static async Task Usage(IWebBrowser chromiumWebBrowser)
#pragma warning restore xUnit1013 // Public method should be marked as test
        {
            #region QuerySelector

            // Add using CefSharp.Dom to access CreateDevToolsContextAsync and related extension methods.
            await using var devToolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

            await devToolsContext.GoToAsync("http://www.google.com");

            // Get element by Id
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/querySelector
            var element = await devToolsContext.QuerySelectorAsync<HtmlElement>("#myElementId");

            //Strongly typed element types (this is only a subset of the types mapped)
            var htmlDivElement = await devToolsContext.QuerySelectorAsync<HtmlDivElement>("#myDivElementId");
            var htmlSpanElement = await devToolsContext.QuerySelectorAsync<HtmlSpanElement>("#mySpanElementId");
            var htmlSelectElement = await devToolsContext.QuerySelectorAsync<HtmlSelectElement>("#mySelectElementId");
            var htmlInputElement = await devToolsContext.QuerySelectorAsync<HtmlInputElement>("#myInputElementId");
            var htmlFormElement = await devToolsContext.QuerySelectorAsync<HtmlFormElement>("#myFormElementId");
            var htmlAnchorElement = await devToolsContext.QuerySelectorAsync<HtmlAnchorElement>("#myAnchorElementId");
            var htmlImageElement = await devToolsContext.QuerySelectorAsync<HtmlImageElement>("#myImageElementId");
            var htmlTextAreaElement = await devToolsContext.QuerySelectorAsync<HtmlImageElement>("#myTextAreaElementId");
            var htmlButtonElement = await devToolsContext.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId");
            var htmlParagraphElement = await devToolsContext.QuerySelectorAsync<HtmlParagraphElement>("#myParagraphElementId");
            var htmlTableElement = await devToolsContext.QuerySelectorAsync<HtmlTableElement>("#myTableElementId");

            // Get a custom attribute value
            var customAttribute = await element.GetAttributeAsync<string>("data-customAttribute");

            //Set innerText property for the element
            await element.SetInnerTextAsync("Welcome!");

            //Get innerText property for the element
            var innerText = await element.GetInnerTextAsync();

            //Get all child elements
            var childElements = await element.QuerySelectorAllAsync("div");

            //Change CSS style background colour
            await element.EvaluateFunctionAsync("e => e.style.backgroundColor = 'yellow'");

            //Type text in an input field
            await element.TypeAsync("Welcome to my Website!");

            //Click The element
            await element.ClickAsync();

            // Simple way of chaining method calls together when you don't need a handle to the HtmlElement
            var htmlButtonElementInnerText = await devToolsContext.QuerySelectorAsync<HtmlButtonElement>("#myButtonElementId")
                .AndThen(x => x.GetInnerTextAsync());

            //Event Handler
            //Expose a function to javascript, functions persist across navigations
            //So only need to do this once
            await devToolsContext.ExposeFunctionAsync("jsAlertButtonClick", () =>
            {
                _ = devToolsContext.EvaluateExpressionAsync("window.alert('Hello! You invoked window.alert()');");
            });

            var jsAlertButton = await devToolsContext.QuerySelectorAsync<HtmlButtonElement>("#jsAlertButton");

            //Write up the click event listner to call our exposed function
            _ = jsAlertButton.AddEventListenerAsync("click", "jsAlertButtonClick");

            //Get a collection of HtmlElements
            var divElements = await devToolsContext.QuerySelectorAllAsync<HtmlDivElement>("div");

            foreach (var div in divElements)
            {
                // Get a reference to the CSSStyleDeclaration
                var style = await div.GetStyleAsync();

                //Set the border to 1px solid red
                await style.SetPropertyAsync("border", "1px solid red", important: true);

                await div.SetAttributeAsync("data-customAttribute", "123");
                await div.SetInnerTextAsync("Updated Div innerText");
            }

            //Using standard array
            var tableRows = await htmlTableElement.GetRowsAsync().ToArrayAsync();

            foreach (var row in tableRows)
            {
                var cells = await row.GetCellsAsync().ToArrayAsync();
                foreach (var cell in cells)
                {
                    var newDiv = await devToolsContext.CreateHtmlElementAsync<HtmlDivElement>("div");
                    await newDiv.SetInnerTextAsync("New Div Added!");
                    await cell.AppendChildAsync(newDiv);
                }
            }

            //Get a reference to the HtmlCollection and use async enumerable
            //Requires Net Core 3.1 or higher
            var tableRowsHtmlCollection = await htmlTableElement.GetRowsAsync();

            await foreach (var row in tableRowsHtmlCollection)
            {
                var cells = await row.GetCellsAsync();
                await foreach (var cell in cells)
                {
                    var newDiv = await devToolsContext.CreateHtmlElementAsync<HtmlDivElement>("div");
                    await newDiv.SetInnerTextAsync("New Div Added!");
                    await cell.AppendChildAsync(newDiv);
                }
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
