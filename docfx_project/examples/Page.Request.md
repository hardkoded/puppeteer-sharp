# How to log network requests
_Contributors: [Meir Blachman](https://github.com/meir017)_

## Problem

You need to monitor the outgoing network requests.

## Solution

Use `Page.Request` event to monitor network requests.


```cs
using var browser = await Puppeteer.LaunchAsync(new () { Headless = true });
var page = await browser.NewPageAsync();
page.Request += (sender, e) =>
{
    Console.WriteLine($"Request: {e.Request.Method} {e.Request.Url}");
    foreach (var header in e.Request.Headers)
    {
        Console.WriteLine($"{header.Key}: {header.Value}");
    }
};
await page.GoToAsync("https://example.com");
```
