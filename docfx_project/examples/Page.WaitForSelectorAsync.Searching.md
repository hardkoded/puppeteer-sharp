# How to wait for a selector

_Contributors: [Bilal Durrani](https://bilaldurrani.io/)_

## Problem

You need to wait for a selector to exist before operating on it.

## Solution

Use `Page.WaitForSelectorAsync()` to delay execution until the selector is available.

```cs
using (var browser = await Puppeteer.LaunchAsync(options))
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("https://developers.google.com/web/");
    // Type into search box.
    await page.TypeAsync("#searchbox input", "Headless Chrome");

    // Wait for suggest overlay to appear and click "show all results".
    var allResultsSelector = ".devsite-suggest-all-results";
    await page.WaitForSelectorAsync(allResultsSelector);
    await page.ClickAsync(allResultsSelector);

    // continue the operation 
} 
```
