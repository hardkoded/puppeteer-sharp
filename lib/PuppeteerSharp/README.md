# Puppeteer Sharp

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer).

## What is Puppeteer Sharp?

Puppeteer Sharp provides a high-level API to control headless Chrome or Chromium over the DevTools Protocol. It can also be configured to use full (non-headless) Chrome or Chromium.

## What can you do?

Most things that you can do manually in the browser can be done using Puppeteer Sharp! Here are a few examples:

* Generate screenshots and PDFs of pages
* Crawl a SPA (Single-Page Application) and generate pre-rendered content (i.e. "SSR" (Server-Side Rendering))
* Automate form submission, UI testing, keyboard input, etc.
* Create an up-to-date, automated testing environment
* Capture a timeline trace of your site to help diagnose performance issues
* Test Chrome Extensions

## Prerequisites

* Puppeteer-Sharp comes in two flavors: a NetStandard 2.0 library for .NET Framework 4.6.1 and .NET Core 2.0 or greater and a .NET 8 version.
* If you have issues running Chrome on Linux, the Puppeteer repo has a [great troubleshooting guide](https://github.com/puppeteer/puppeteer/blob/master/docs/troubleshooting.md).
* X-server is required on Linux.

## Usage

### Take screenshots

```csharp
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(
    new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile);
```

### Generate PDF files

```csharp
var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.EvaluateExpressionHandleAsync("document.fonts.ready"); // Wait for fonts to be loaded
await page.PdfAsync(outputFile);
```

### Evaluate Javascript

```csharp
await using var page = await browser.NewPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
var someObject = await page.EvaluateFunctionAsync<JsonElement>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.GetProperty("a").GetString());
```

## Useful links

* [API Documentation](http://www.puppeteersharp.com/api/index.html)
* [GitHub Repository](https://github.com/hardkoded/puppeteer-sharp)
* [StackOverflow](https://stackoverflow.com/search?q=puppeteer-sharp)
* [Slack channel #puppeteer-sharp](https://www.hardkoded.com/goto/pptr-slack)

## Support

If you have an issue or a question:

* Ask a question on [Stack Overflow](https://stackoverflow.com/search?q=puppeteer-sharp)
* File a [new issue](https://github.com/hardkoded/puppeteer-sharp/issues/new)

## Contributing

Check out [contributing guide](https://github.com/hardkoded/puppeteer-sharp/blob/master/CONTRIBUTING.md) to get an overview of Puppeteer Sharp development.
