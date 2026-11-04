---
title: "IBrowserContext"
description: "BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has a single IBrowserContext used by default...."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowserContext"
  hidden: true
---

<a id="PuppeteerSharp_IBrowserContext"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowserContext : IDisposable, IAsyncDisposable
```


BrowserContexts provide a way to operate multiple independent browser sessions. When a browser is launched, it has
a single [IBrowserContext](./) used by default. The method [NewPageAsync(CreatePageOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_NewPageAsync_PuppeteerSharp_CreatePageOptions_) creates a [IPage](/api/PuppeteerSharp/IPage/) in the default [IBrowserContext](./).

**Derived:** [BidiBrowserContext](/api/PuppeteerSharp.Bidi/BidiBrowserContext/), [BrowserContext](/api/PuppeteerSharp/BrowserContext/), [CdpBrowserContext](/api/PuppeteerSharp.Cdp/CdpBrowserContext/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserContext.cs#L11)
