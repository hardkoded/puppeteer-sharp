# Puppeteer Sharp - Examples

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/GoogleChrome/puppeteer).

# Basic Usage

## Take screenshots

```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
});
var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.ScreenshotAsync(outputFile);
```

You can also change the view port before generating the screenshot

```cs
await page.SetViewportAsync(new ViewPortOptions
{
    Width = 500,
    Height = 500
});
```

## Generate PDF files

```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync();
var browser = await Puppeteer.LaunchAsync(new LaunchOptions
{
    Headless = true
});
var page = await browser.NewPageAsync();
await page.GoToAsync("http://www.google.com");
await page.PdfAsync(outputFile);
```

## Get Inner Text of an Element

```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("https://www.hardkoded.com/blog/ui-testing-with-puppeteer-released");
    var pageHeaderHandle = await page.QuerySelectorAsync("h1");
    var innerTextHandle = await pageHeaderHandle.GetPropertyAsync("innerText");
    var innerText = await innerTextHandle.JsonValueAsync();
    // Inner Text: UI testing with Puppeteer book officially released!
}
```

## Inject HTML

```cs
using(var page = await browser.NewPageAsync())
{
    await page.SetContentAsync("<div>My Receipt</div>");
    var result = await page.GetContentAsync();
    await page.PdfAsync(outputFile);
    SaveHtmlToDB(result);
}
```

## Evaluate Javascript

```cs
using (var page = await browser.NewPageAsync())
{
    var seven = await page.EvaluateExpressionAsync<int>("()=> 4 + 3");
    var someObject = await page.EvaluateFunctionAsync<dynamic>("(value) => ({a: value})", 5);
    Console.WriteLine(someObject.a);
}
```

## Wait For Selector

```cs
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    await page.WaitForSelectorAsync("div.main-content")
    await page.PdfAsync(outputFile));
}
```

## Wait For Function

```cs
using (var page = await Browser.NewPageAsync())
{
    await page.GoToAsync("http://www.spapage.com");
    var watchDog = page.WaitForFunctionAsync(" () => window.innerWidth < 100");
    await page.SetViewportAsync(new ViewPortOptions { Width = 50, Height = 50 });
    await watchDog;
}
```

## Connect to a remote browser

```cs
var options = new ConnectOptions()
{
    BrowserWSEndpoint = $"wss://www.externalbrowser.io?token={apikey}"
};

var url = "https://www.google.com/";

using (var browser = await PuppeteerSharp.Puppeteer.ConnectAsync(options))
{
    using (var page = await browser.NewPageAsync())
    {
        await page.GoToAsync(url);
        await page.PdfAsync("wot.pdf");
    }
}
```
