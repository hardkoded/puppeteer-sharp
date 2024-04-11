# How to test a Chrome Extension
_Contributors: [Dario Kondratiuk](https://github.com/kblok)_

## Problem

You need to download an use an specific browser version.

## Solution

Thank to [Google for testing](https://developer.chrome.com/blog/chrome-for-testing). Now finding and downloading a specific version of Chrome is easy.

You will find the available versions [here](https://googlechromelabs.github.io/chrome-for-testing/known-good-versions.json).
Once you have the version you want, you can download it using the `BrowserFetcher` class.

```cs
<!-- snippet: CustomVersionsExample -->
<a id='snippet-CustomVersionsExample'></a>
```cs
Console.WriteLine("Downloading browsers");

using var browserFetcher = new BrowserFetcher(SupportedBrowser.Chrome);
var chrome118 = await browserFetcher.DownloadAsync("118.0.5993.70");
var chrome119 = await browserFetcher.DownloadAsync("119.0.5997.0");

Console.WriteLine("Navigating");
await using (var browser = await Puppeteer.LaunchAsync(new()
{
    ExecutablePath = chrome118.GetExecutablePath(),
}))
{
    await using var page = await browser.NewPageAsync();
    await page.GoToAsync("https://www.whatismybrowser.com/");

    Console.WriteLine("Generating PDF");
    await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "118.pdf"));

    Console.WriteLine("Export completed");
}

await using (var browser = await Puppeteer.LaunchAsync(new()
{
    ExecutablePath = chrome119.GetExecutablePath(),
}))
{
    await using var page = await browser.NewPageAsync();
    await page.GoToAsync("https://www.whatismybrowser.com/");

    Console.WriteLine("Generating PDF");
    await page.PdfAsync(Path.Combine(Directory.GetCurrentDirectory(), "119.pdf"));

    Console.WriteLine("Export completed");
}
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/Browsers/Chrome/ChromeDataTests.cs#L14-L49' title='Snippet source file'>snippet source</a> | <a href='#snippet-CustomVersionsExample' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->
```
