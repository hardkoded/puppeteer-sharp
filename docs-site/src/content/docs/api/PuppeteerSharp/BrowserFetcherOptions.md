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

The default download uses [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient), which trusts the certificates of the operating system.
If browser downloads must trust custom certificates (for example behind a corporate proxy), install the certificate
in the system store (on Linux you can also point the SSL_CERT_FILE or SSL_CERT_DIR environment variables to it),
or provide a delegate here that downloads the file with an [HttpClient](https://learn.microsoft.com/dotnet/api/system.net.http.httpclient) configured with your certificates.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L61)

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

<a id="PuppeteerSharp_BrowserFetcherOptions_Retry_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_Retry"></a>

### Retry


```csharp
public int Retry { get; set; }
```


Number of retries to attempt if downloading the browser archive fails due to transient network or server errors
(HTTP 408, 429, 5xx, or network drops, with 1s–10s exponential backoff). Defaults to 0.
Only applies to the default download; it is ignored when [CustomFileDownload](#PuppeteerSharp_BrowserFetcherOptions_CustomFileDownload) is set.
Mirrors upstream's `retry` install option.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L50)

<a id="PuppeteerSharp_BrowserFetcherOptions_SkipChromeHeadlessShellDownload_"></a>
<a id="PuppeteerSharp_BrowserFetcherOptions_SkipChromeHeadlessShellDownload"></a>

### SkipChromeHeadlessShellDownload


```csharp
public bool SkipChromeHeadlessShellDownload { get; set; }
```


Whether to skip downloading Chrome Headless Shell when downloading Chrome. Defaults to false.
Mirrors upstream's `skipChromeHeadlessShellDownload` configuration.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcherOptions.cs#L42)
