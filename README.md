# CefSharp.DevTools.Dom (Formerly CefSharp.Puppeteer)

[![Nuget](https://img.shields.io/nuget/v/CefSharp.DevTools.Dom?style=for-the-badge)](https://www.nuget.org/packages/CefSharp.DevTools.Dom/)
[![AppVeyor](https://img.shields.io/appveyor/build/cefsharp/CefSharp.DevTools.Dom?style=for-the-badge)](https://ci.appveyor.com/project/cefsharp/CefSharp.DevTools.Dom)
[![AppVeyor tests](https://img.shields.io/appveyor/tests/cefsharp/CefSharp.DevTools.Dom?style=for-the-badge)](https://ci.appveyor.com/project/cefsharp/CefSharp.DevTools.Dom/build/tests)
[![GitHub](https://img.shields.io/github/license/cefsharp/CefSharp.DevTools.Dom?style=for-the-badge)](https://github.com/cefsharp/CefSharp.DevTools.Dom/blob/main/LICENSE)

CefSharp.DevTools.Dom is a fork of [puppeteer-sharp by Darío Kondratiuk](https://github.com/hardkoded/puppeteer-sharp) that has been adapted specifically for use with CefSharp.
- Strongly typed async DOM API
- Direct communication with the ChromiumWebBrowser instance rather than opening a web socket.
- 1:1 mapping of DevToolsContext and ChromiumWebBrowser
- CEF only supports a subset of features, features will be added/removed as the project matures

# Prerequisites

 * .Net 4.7.2 or .Net Core 3.1 or greater
 * CefSharp 102.0.100 or greater

# Questions and Support

If you have an issue or a question:

* Ask a question on [Discussions](https://github.com/cefsharp/CefSharp.DevTools.Dom/discussions).

## Contributing Guide

See [this document](CONTRIBUTING.md) for information on how to contribute.

# Usage

## DevToolsContext

The **DevToolsContext** class is the main entry point into the library and can be created from a
ChromiumWebBrowser instance.
Only a **single** DevToolsContext should exist at any given time, when you are finished them make sure you
dispose via DisposeAsync.
Starting in version 2.x the **DevToolsContext** multiple calls to CreateDevToolsContextAsync will return the same
instance per ChromiumWebBrowser and will be Disposed when the ChromiumWebBrowser is Disposed. If you need to use
the DevToolsContext in multiple places in your code, calling CreateDevToolsContextAsync is now supported without dispoing.
If the DevToolsContext is disposed then calls to CreateDevToolsContextAsync will create a new instance.

```c#
// Add using CefSharp.DevTools.Dom; to get access to the
// CreateDevToolsContextAsync extension method
var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

// Manually dispose of context (prefer DisposeAsync as the whole API is async)
await devToolsContext.DisposeAsync();
```

```c#
// Dispose automatically via await using
// https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-disposeasync#using-async-disposable
await using var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();
```

## DOM Access

Read/write to the DOM
<!-- snippet: QuerySelector -->
<a id='snippet-queryselector'></a>
```cs
// Add using CefSharp.DevTools.Dom to access CreateDevToolsContextAsync and related extension methods.
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
```
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/DevToolsContextQuerySelectorTests.cs#L22-L128' title='Snippet source file'>snippet source</a> | <a href='#snippet-queryselector' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Inject HTML
<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
//Wait for Initial page load
await chromiumWebBrowser.WaitForInitialLoadAsync();

await using var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();
await devtoolsContext.SetContentAsync("<div>My Receipt</div>");
var result = await devtoolsContext.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/DevToolsContextTests/SetContentTests.cs#L25-L33' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-evaluate'></a>
```cs
await using var devtoolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();
var seven = await devtoolsContext.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await devtoolsContext.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
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

await using var devToolsContext = await chromiumWebBrowser.CreateDevToolsContextAsync();

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

## Generate PDF files

Currently not supported via CefSharp.DevTools.Dom, use ChromiumWebBrowser.PrintToPdfAsync instead.
