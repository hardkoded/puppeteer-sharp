---
title: "IBrowserFetcher"
description: "BrowserFetcher can download and manage different versions of Chromium."
editUrl: false
prev: false
next: false
sidebar:
  label: "IBrowserFetcher"
  hidden: true
---

<a id="PuppeteerSharp_IBrowserFetcher"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IBrowserFetcher
```


BrowserFetcher can download and manage different versions of Chromium.

**Derived:** [BrowserFetcher](/api/PuppeteerSharp/BrowserFetcher/)

**Example**

Example on how to use BrowserFetcher to download a specific version of Chromium and run Puppeteer against it:


```csharp
var browserFetcher = Puppeteer.CreateBrowserFetcher();
var revisionInfo = await browserFetcher.DownloadAsync(BrowserData.Chrome.DefaultBuildId);
var browser = await Puppeteer.LaunchAsync(new LaunchOptions { ExecutablePath = revisionInfo.ExecutablePath});
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L19)
