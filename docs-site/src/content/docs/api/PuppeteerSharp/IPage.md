---
title: "IPage"
description: "Provides methods to interact with a single tab in Chromium. One IBrowser instance might have multiple IPage instances."
editUrl: false
prev: false
next: false
sidebar:
  label: "IPage"
  hidden: true
---

<a id="PuppeteerSharp_IPage"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IPage : IDisposable, IAsyncDisposable
```


Provides methods to interact with a single tab in Chromium. One [IBrowser](/api/PuppeteerSharp/IBrowser/) instance might have multiple [IPage](./) instances.

**Derived:** [BidiPage](/api/PuppeteerSharp.Bidi/BidiPage/), [CdpPage](/api/PuppeteerSharp.Cdp/CdpPage/), [Page](/api/PuppeteerSharp/Page/)

**Example**

This example creates a page, navigates it to a URL, and then saves a screenshot:


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await page.ScreenshotAsync("screenshot.png");
await browser.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IPage.cs#L27)
