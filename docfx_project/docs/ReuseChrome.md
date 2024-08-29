# How to download and reuse Chrome from a custom location

_Contributors: [Bilal Durrani](https://bilaldurrani.io/)_

## Problem

You want to download Chrome in a custom folder and you want to reuse Chrome
from a location where it was previously downloaded instead of from the default location.

## Solution

Use `BrowserFetcherOptions` to specify the full path for where to download Chrome.

<!-- snippet: ReuseChromeExample -->
<a id='snippet-ReuseChromeExample'></a>
```cs
var downloadPath = "/Users/dario/chrome";
var browserFetcherOptions = new BrowserFetcherOptions { Path = downloadPath };
var browserFetcher = new BrowserFetcher(browserFetcherOptions);
var installedBrowser = await browserFetcher.DownloadAsync();
```
<sup><a href='https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp.Tests/Browsers/Chrome/ChromeDataTests.cs#L14-L19' title='Snippet source file'>snippet source</a> | <a href='#snippet-ReuseChromeExample' title='Start of snippet'>anchor</a></sup>
<!-- endSnippet -->

Use `Puppeteer.LaunchAsync()` with `LaunchOptions` with the `LaunchOptions.ExecutablePath` property set to the
fully qualified path to the Chrome executable.

```
var options = new LaunchOptions { Headless = true, ExecutablePath = installedBrowser.GetExecutablePath() };

await using var browser = await Puppeteer.LaunchAsync(options);
await using var page = await browser.NewPageAsync();
await page.GoToAsync("about:blank");
```
