# Puppeteer Sharp - Examples

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer).

# Basic Usage

## Take screenshots

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com");
await page.ScreenshotAsync("screenshot.png");
```

You can also change the viewport before generating the screenshot:

```cs
await page.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```

## Generate PDF files

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com");
await page.PdfAsync("output.pdf");
```

## Use Locators

Locators provide a smart way to find elements with built-in auto-retry and auto-wait:

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");

// Click a button with auto-wait
await page.Locator("button.submit").ClickAsync();

// Fill an input
await page.Locator("input[name='email']").FillAsync("user@example.com");
```

See the [Locators guide](Locators.md) for more details.

## Get Inner Text of an Element

```cs
using var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
var pageHeaderHandle = await page.QuerySelectorAsync("h1");
var innerTextHandle = await pageHeaderHandle.GetPropertyAsync("innerText");
var innerText = await innerTextHandle.JsonValueAsync();
```

## Inject HTML

```cs
using var page = await browser.NewPageAsync();
await page.SetContentAsync("<div>My Receipt</div>");
var result = await page.GetContentAsync();
await page.PdfAsync("output.pdf");
```

## Evaluate Javascript

```cs
using var page = await browser.NewPageAsync();
var seven = await page.EvaluateExpressionAsync<int>("() => 4 + 3");
var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
Console.WriteLine(someObject.a);
```

## Wait For Selector

```cs
using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.spapage.com");
await page.WaitForSelectorAsync("div.main-content");
await page.PdfAsync("output.pdf");
```

## Wait For Function

```cs
using var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.spapage.com");
var watchDog = page.WaitForFunctionAsync("() => window.innerWidth < 100");
await page.SetViewportAsync(new ViewPortOptions { Width = 50, Height = 50 });
await watchDog;
```

## Connect to a remote browser

```cs
var options = new ConnectOptions()
{
    BrowserWSEndpoint = $"wss://www.externalbrowser.io?token={apikey}"
};

var url = "https://www.google.com/";

using var browser = await Puppeteer.ConnectAsync(options);
using var page = await browser.NewPageAsync();
await page.GoToAsync(url);
await page.PdfAsync("output.pdf");
```
