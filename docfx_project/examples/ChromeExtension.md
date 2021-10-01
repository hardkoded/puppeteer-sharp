# How to test a Chrome Extension
_Contributors: [Meir Blachman](https://github.com/meir017)_

## Problem

You need to test a chrome extension

## Solution

Use `Puppeteer.LaunchAsync` passing arguments specifying to load your extension.

```cs
using var browserFetcher = new BrowserFetcher();
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);

var pathToExtension = "path/to/extension";
var launchOptions = new LaunchOptions()
{
    Headless = false,
    Args = new []
    {
        $@"--disable-extensions-except=""{pathToExtension}""",
        $@"--load-extension=""{pathToExtension}"""
    }
};

using (var browser = await Puppeteer.LaunchAsync(launchOptions))
using (var page = await browser.NewPageAsync())
{
    // test your extension here
}
```
