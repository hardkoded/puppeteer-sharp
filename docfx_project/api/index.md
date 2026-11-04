# Puppeteer Sharp API

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer).

# Usage

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

### Generate PDF files with custom options

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com");
await page.PdfAsync("output.pdf", new PdfOptions
{
    Format = PaperFormat.A4,
    DisplayHeaderFooter = true,
    MarginOptions = new MarginOptions
    {
        Top = "20px",
        Right = "20px",
        Bottom = "40px",
        Left = "20px"
    },
    FooterTemplate = "<div id=\"footer-template\" style=\"font-size:10px !important; color:#808080; padding-left:10px\">Footer Text</div>"
});
```

## Use Locators

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await page.Locator("button.submit").ClickAsync();
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
var seven = await page.EvaluateExpressionAsync<int>("4 + 3");
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
