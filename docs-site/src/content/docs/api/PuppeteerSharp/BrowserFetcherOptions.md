---
title: "BrowserFetcherOptions"
description: "Browser fetcher options used to construct a BrowserFetcher."
editUrl: false
prev: false
next: false
sidebar:
  label: "BrowserFetcherOptions"
  hidden: true
---

<a id="PuppeteerSharp_BrowserFetcherOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class BrowserFetcherOptions
```


Browser fetcher options used to construct a [BrowserFetcher](/api/PuppeteerSharp/BrowserFetcher/).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L8)

## Properties

<a id="PuppeteerSharp_BrowserFetcherOptions_Browser_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_Browser"></a>

### Browser


```csharp
public SupportedBrowser Browser { get; set; }
```


Browser. Defaults to Chrome.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L21)

<a id="PuppeteerSharp_BrowserFetcherOptions_CustomFileDownload_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_CustomFileDownload"></a>

### CustomFileDownload


```csharp
public BrowserFetcherOptions.CustomFileDownloadAction CustomFileDownload { get; set; }
```


Gets the default or a custom download delegate.

**Value:** [BrowserFetcherOptions.CustomFileDownloadAction](/api/PuppeteerSharp/BrowserFetcherOptions.CustomFileDownloadAction/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L41)

<a id="PuppeteerSharp_BrowserFetcherOptions_Host_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_Host"></a>

### Host


```csharp
public string Host { get; set; }
```


A download host to be used. Defaults to https://storage.googleapis.com.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L36)

<a id="PuppeteerSharp_BrowserFetcherOptions_Path_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


A path for the downloads folder. Defaults to [root]/.local-chromium, where [root] is where the project binaries are located.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L31)

<a id="PuppeteerSharp_BrowserFetcherOptions_Platform_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_Platform"></a>

### Platform


```csharp
public Platform? Platform { get; set; }
```


Platform. Defaults to current platform.

**Value:** `Platform?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L26)
