---
title: "BrowserContextOptions"
description: "BrowserContext options."
editUrl: false
prev: false
next: false
sidebar:
  label: "BrowserContextOptions"
  hidden: true
---

<a id="PuppeteerSharp_BrowserContextOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class BrowserContextOptions
```


BrowserContext options.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContextOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_BrowserContextOptions_DownloadBehavior_"></a>
<a id="PuppeteerSharp_BrowserContextOptions_DownloadBehavior"></a>

### DownloadBehavior


```csharp
public DownloadBehavior DownloadBehavior { get; set; }
```


Behavior definition for when downloading a file.

**Value:** [DownloadBehavior](/api/PuppeteerSharp/DownloadBehavior/)

If not set, the default behavior will be used.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContextOptions.cs#L25)

<a id="PuppeteerSharp_BrowserContextOptions_ProxyBypassList_"></a>
<a id="PuppeteerSharp_BrowserContextOptions_ProxyBypassList"></a>

### ProxyBypassList


```csharp
public string[] ProxyBypassList { get; set; }
```


Bypass the proxy for the given semi-colon-separated list of hosts.

**Value:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContextOptions.cs#L17)

<a id="PuppeteerSharp_BrowserContextOptions_ProxyServer_"></a>
<a id="PuppeteerSharp_BrowserContextOptions_ProxyServer"></a>

### ProxyServer


```csharp
public string ProxyServer { get; set; }
```


Proxy server with optional port to use for all requests.
Username and password can be set in [AuthenticateAsync(Credentials)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_AuthenticateAsync_PuppeteerSharp_Credentials_).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserContextOptions.cs#L12)
