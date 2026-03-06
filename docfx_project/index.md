# Browser Automation for .NET

Puppeteer Sharp is a .NET port of the official [Node.JS Puppeteer API](https://github.com/puppeteer/puppeteer). Control Chrome and Firefox with a high-level, easy-to-use API.

## Quick Start

```
dotnet add package PuppeteerSharp
```

```cs
using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await page.ScreenshotAsync("screenshot.png");
```

## Key Features

### Browser Automation

Automate Chrome and Firefox with full support for navigation, input, network interception, and more. Works with both headless and headful modes.

### Screenshots & PDFs

Generate pixel-perfect screenshots and PDF documents. Supports full-page capture, custom viewports, clipping, and PDF formatting options.

### Locators

Smart element location with built-in auto-retry and auto-wait. Configure visibility, timeouts, and preconditions with a fluent API.

```cs
await page.Locator("button.submit")
    .SetWaitForEnabled(true)
    .SetEnsureElementIsInTheViewport(true)
    .ClickAsync();
```

### Dual Protocol Support

First-class support for both Chrome DevTools Protocol (CDP) and WebDriver BiDi, giving you flexibility to choose the best protocol for your use case.

### Cross-Platform

Runs on Windows, macOS, and Linux. Targets .NET Standard 2.0 and .NET 10, so it works with .NET Framework 4.6.1+, .NET Core, and modern .NET.

## Sponsor

If you are making money using Puppeteer-Sharp, consider [sponsoring this project](https://github.com/sponsors/hardkoded). This will give you priority support and help this community-based project keep moving.

## Links

* [GitHub](https://github.com/hardkoded/puppeteer-sharp)
* [Slack - #puppeteer-sharp](https://www.hardkoded.com/goto/pptr-slack)
* [StackOverflow](https://stackoverflow.com/search?q=puppeteer-sharp)
* [Issues](https://github.com/hardkoded/puppeteer-sharp/issues)
