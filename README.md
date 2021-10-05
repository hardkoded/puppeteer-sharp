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

## Take screenshots

You can also change the view port before generating the screenshot when using WinForms

<!-- snippet: SetViewportAsync -->
<a id='snippet-setviewportasync'></a>
```cs
await Page.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```
<sup><a href='/lib/PuppeteerSharp.Tests/ScreenshotTests/ElementHandleScreenshotTests.cs#L22-L28' title='Snippet source file'>snippet source</a> | <a href='#snippet-setviewportasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Generate PDF files

Currently not supported via CefSharp Puppeteer, use ChromiumWebBrowser.PrintToPdfAsync instead.

## Inject HTML

<!-- snippet: SetContentAsync -->
<a id='snippet-setcontentasync'></a>
```cs
await using var page = await chromiumWebBrowser.GetPuppeteerPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
```
<sup><a href='/lib/PuppeteerSharp.Tests/PageTests/SetContentTests.cs#L23-L29' title='Snippet source file'>snippet source</a> | <a href='#snippet-setcontentasync' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

## Evaluate Javascript

<!-- snippet: Evaluate -->
<a id='snippet-evaluate'></a>
```cs
await using var page = await chromiumWebBrowser.GetPuppeteerPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```
<sup><a href='/lib/PuppeteerSharp.Tests/QuerySelectorTests/ElementHandleQuerySelectorEvalTests.cs#L21-L26' title='Snippet source file'>snippet source</a> | <a href='#snippet-evaluate' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
