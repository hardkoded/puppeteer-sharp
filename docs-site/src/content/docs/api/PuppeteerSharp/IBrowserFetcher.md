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

## Properties

<a id="PuppeteerSharp_IBrowserFetcher_BaseUrl_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_BaseUrl"></a>

### BaseUrl


```csharp
string BaseUrl { get; set; }
```


A download host to be used.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L24)

<a id="PuppeteerSharp_IBrowserFetcher_Browser_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_Browser"></a>

### Browser


```csharp
SupportedBrowser Browser { get; set; }
```


Gets the browser.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L39)

<a id="PuppeteerSharp_IBrowserFetcher_CacheDir_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_CacheDir"></a>

### CacheDir


```csharp
string CacheDir { get; set; }
```


Determines the path to download browsers to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L29)

<a id="PuppeteerSharp_IBrowserFetcher_Platform_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_Platform"></a>

### Platform


```csharp
Platform Platform { get; set; }
```


Gets the platform.

**Value:** [Platform](/api/PuppeteerSharp/Platform/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L34)

<a id="PuppeteerSharp_IBrowserFetcher_WebProxy_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_WebProxy"></a>

### WebProxy


```csharp
IWebProxy WebProxy { get; set; }
```


Proxy used by the HttpClient in [DownloadAsync()](#PuppeteerSharp_IBrowserFetcher_DownloadAsync), [DownloadAsync(string)](#PuppeteerSharp_IBrowserFetcher_DownloadAsync_System_String_) and [CanDownloadAsync(string)](#PuppeteerSharp_IBrowserFetcher_CanDownloadAsync_System_String_).

**Value:** [IWebProxy](https://learn.microsoft.com/dotnet/api/system.net.iwebproxy)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L44)

## Methods

<a id="PuppeteerSharp_IBrowserFetcher_CanDownloadAsync_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_CanDownloadAsync_System_String_"></a>

### CanDownloadAsync(string)


```csharp
Task<bool> CanDownloadAsync(string buildId)
```


The method initiates a HEAD request to check if the revision is available.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A build to check availability. |

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Whether the version is available or not.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L51)

<a id="PuppeteerSharp_IBrowserFetcher_DownloadAsync_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_DownloadAsync"></a>

### DownloadAsync()


```csharp
Task<InstalledBrowser> DownloadAsync()
```


Downloads the revision.

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L57)

<a id="PuppeteerSharp_IBrowserFetcher_DownloadAsync_PuppeteerSharp_BrowserTag_"></a>

### DownloadAsync(BrowserTag)


```csharp
Task<InstalledBrowser> DownloadAsync(BrowserTag tag)
```


Downloads the revision.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `tag` | [BrowserTag](/api/PuppeteerSharp/BrowserTag/) | Browser tag. |

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L64)

<a id="PuppeteerSharp_IBrowserFetcher_DownloadAsync_System_String_"></a>

### DownloadAsync(string)


```csharp
Task<InstalledBrowser> DownloadAsync(string revision)
```


Downloads the revision.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `revision` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Revision. |

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L71)

<a id="PuppeteerSharp_IBrowserFetcher_GetExecutablePath_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_GetExecutablePath_System_String_"></a>

### GetExecutablePath(string)


```csharp
string GetExecutablePath(string buildId)
```


Gets the executable path.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Browser buildId. |

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The executable path.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L90)

<a id="PuppeteerSharp_IBrowserFetcher_GetInstalledBrowsers_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_GetInstalledBrowsers"></a>

### GetInstalledBrowsers()


```csharp
IEnumerable<InstalledBrowser> GetInstalledBrowsers()
```


A list of all browsers available locally on disk.

**Returns:** [IEnumerable&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1) — The available browsers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L77)

<a id="PuppeteerSharp_IBrowserFetcher_Uninstall_"></a>
<a id="PuppeteerSharp_IBrowserFetcher_Uninstall_System_String_"></a>

### Uninstall(string)


```csharp
void Uninstall(string buildId)
```


Removes a downloaded browser.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Browser to remove. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IBrowserFetcher.cs#L83)
