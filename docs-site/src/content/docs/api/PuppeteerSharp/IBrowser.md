---
title: "IBrowser"
description: "Provides methods to interact with a browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowser"
  hidden: true
---

<a id="PuppeteerSharp_IBrowser"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowser : IDisposable, IAsyncDisposable
```


Provides methods to interact with a browser.

**Derived:** [BidiBrowser](/api/PuppeteerSharp.Bidi/BidiBrowser/), [Browser](/api/PuppeteerSharp/Browser/), [CdpBrowser](/api/PuppeteerSharp.Cdp/CdpBrowser/)

**Example**

An example of using a [IBrowser](./) to create a [IPage](/api/PuppeteerSharp/IPage/):


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://example.com");
await browser.CloseAsync();
```


An example of disconnecting from and reconnecting to a [Browser](/api/PuppeteerSharp/Browser/):


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var browserWSEndpoint = browser.WebSocketEndpoint;
browser.Disconnect();
var browser2 = await Puppeteer.ConnectAsync(new ConnectOptions { BrowserWSEndpoint = browserWSEndpoint });
await browser2.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowser.cs#L33)
