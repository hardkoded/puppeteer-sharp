# How to all the links from a page

_Contributors: [Bilal Durrani](https://bilaldurrani.io/)_

## Problem

You need to get all links from a page.

## Solution

Use `Page.EvaluateExpressionAsync` to evaluate javascript in the context of the browser
and return the `href` associated with the hyperlink tag.

```cs
using (var browser = await Puppeteer.LaunchAsync(options))
using (var page = await browser.NewPageAsync())
{
    await page.GoToAsync("http://www.google.com");
    var jsSelectAllAnchors = @"Array.from(document.querySelectorAll('a')).map(a => a.href);";
    var urls = await page.EvaluateExpressionAsync<string[]>(jsSelectAllAnchors);
    foreach (string url in urls)
    {
        Console.WriteLine($"Url: {url}");
    }
    Console.WriteLine("Press any key to continue...");
    Console.ReadLine();
}
```
