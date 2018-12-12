# How to download and reuse Chrome from a custom location

_Contributors: [Bilal Durrani](https://bilaldurrani.io/)_

## Problem

You want to download Chrome in a custom folder and you want to reuse Chrome
from a location where it was previously downloaded instead of from the default location. 

## Solution

Use `BrowserFetcherOptions` to specify the full path for where to download Chrome.

```
var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
var browserFetcher = new BrowserFetcher(browserFetcherOptions);
await browserFetcher.DownloadAsync(BrowserFetcher.DefaultRevision);
```

Use `Puppeteer.LaunchAsync()` with `LaunchOptions` with the `LaunchOptions.ExecutablePath` property set to the
fully qualified path to the Chrome executable.

```
var options = new LaunchOptions { Headless = true, ExecutablePath = executablePath };

using (var browser = await Puppeteer.LaunchAsync(options))
using (var page = await browser.NewPageAsync())
{
    // use page
}
```