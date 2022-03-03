# CefSharp Puppeteer

CefSharp Puppeteer is a fork of [puppeteer-sharp by Darío Kondratiuk](https://github.com/hardkoded/puppeteer-sharp) that has been adapted specifically for use with CefSharp.
Direct communication with the ChromiumWebBrowser instance rather than opening a web socket.
1:1 mapping of Page and ChromiumWebBrowser
CEF only supports a subset of features, features will be added/removed as the project matures

# Prerequisites

 * .Net 4.7.2 or .Net Core 3.1 or greater
 * CefSharp 95.7.141 or greater

# Questions and Support

If you have an issue or a question:

* Ask a question on [Discussions](https://github.com/cefsharp/Puppeteer/discussions).

## Contributing Guide

See [this document](CONTRIBUTING.md) for information on how to contribute.

# Usage

## Generate PDF files

Currently not supported via CefSharp Puppeteer, use ChromiumWebBrowser.PrintToPdfAsync instead.

## DOM Access

Read/write to the DOM
<!-- snippet: QuerySelector -->
<a id='snippet-queryselector'></a>
```cs
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

//Change CSS style background colour
_ = await element.EvaluateFunctionAsync("e => e.style.backgroundColor = 'yellow'");

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
```
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/PageQuerySelectorTests.cs#L22-L57' title='Snippet source file'>snippet source</a> | <a href='#snippet-queryselector' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Inject HTML
<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
//Wait for Initial page load
await chromiumWebBrowser.WaitForInitialLoadAsync();

await using var devtoolsContext = await chromiumWebBrowser.GetDevToolsContextAsync();
await devtoolsContext.SetContentAsync("<div>My Receipt</div>");
var result = await devtoolsContext.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L25-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-evaluate'></a>
```cs
await using var page = await chromiumWebBrowser.GetDevToolsContextAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/ElementHandleQuerySelectorEvalTests.cs#L21-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-evaluate' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Take screenshots

<!-- snippet: Screenshot -->
<a id='snippet-screenshot'></a>
```cs
//Wait for Initial page load
await chromiumWebBrowser.WaitForInitialLoadAsync();

await using var devToolsContext = await chromiumWebBrowser.GetDevToolsContextAsync();

await devToolsContext.ScreenshotAsync("file.png");
```
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L23-L30' title='Snippet source file'>snippet source</a> | <a href='#snippet-screenshot' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

<!-- snippet: SetViewportAsync -->
<a id='snippet-setviewportasync'></a>
```cs
// Set Viewport
await DevToolsContext.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L37-L44' title='Snippet source file'>snippet source</a> | <a href='#snippet-setviewportasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->


