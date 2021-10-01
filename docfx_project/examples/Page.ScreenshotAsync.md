# How to take screenshots
_Contributors: [Darío Kondratiuk](https://www.hardkoded.com)_

## Problem

You need to take an screenshot of a page.

## Solution

Use `Page.ScreenshotAsync` passing a file path as an argument.

```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

var url = "https://www.somepage.com";
var file = ".\\somepage.jpg";

var launchOptions = new LaunchOptions()
{
    Headless = false
};

using (var browser = await Puppeteer.LaunchAsync(launchOptions))
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync(url);
    await page.ScreenshotAsync(file);
}
```
