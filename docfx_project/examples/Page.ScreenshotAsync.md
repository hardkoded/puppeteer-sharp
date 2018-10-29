# How to take screenshots

## Problem

You need to take an screenshot of a page.

## Solution

Use `Page.ScreenshotAsync` passing a file path as an argument.

```cs
new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

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