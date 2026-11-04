---
title: "BrowserFetcher"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BrowserFetcher"
  hidden: true
---

<a id="PuppeteerSharp_BrowserFetcher"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public sealed class BrowserFetcher : IBrowserFetcher
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IBrowserFetcher](/api/PuppeteerSharp/IBrowserFetcher/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L21)

## Constructors

<a id="PuppeteerSharp_BrowserFetcher__ctor_"></a>
<a id="PuppeteerSharp_BrowserFetcher__ctor"></a>

### BrowserFetcher()


```csharp
public BrowserFetcher()
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L29)

<a id="PuppeteerSharp_BrowserFetcher__ctor_PuppeteerSharp_BrowserFetcherOptions_Microsoft_Extensions_Logging_ILoggerFactory_"></a>

### BrowserFetcher(BrowserFetcherOptions, ILoggerFactory)


```csharp
public BrowserFetcher(BrowserFetcherOptions options, ILoggerFactory loggerFactory = null)
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [BrowserFetcherOptions](/api/PuppeteerSharp/BrowserFetcherOptions/) |  |
| `loggerFactory` | [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L44)

<a id="PuppeteerSharp_BrowserFetcher__ctor_PuppeteerSharp_SupportedBrowser_Microsoft_Extensions_Logging_ILoggerFactory_"></a>

### BrowserFetcher(SupportedBrowser, ILoggerFactory)


```csharp
public BrowserFetcher(SupportedBrowser browser, ILoggerFactory loggerFactory = null)
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `browser` | [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/) |  |
| `loggerFactory` | [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L38)

## Properties

<a id="PuppeteerSharp_BrowserFetcher_BaseUrl_"></a>
<a id="PuppeteerSharp_BrowserFetcher_BaseUrl"></a>

### BaseUrl


```csharp
public string BaseUrl { get; set; }
```


A download host to be used.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L62)

<a id="PuppeteerSharp_BrowserFetcher_Browser_"></a>
<a id="PuppeteerSharp_BrowserFetcher_Browser"></a>

### Browser


```csharp
public SupportedBrowser Browser { get; set; }
```


Gets the browser.

**Value:** [SupportedBrowser](/api/PuppeteerSharp/SupportedBrowser/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L68)

<a id="PuppeteerSharp_BrowserFetcher_CacheDir_"></a>
<a id="PuppeteerSharp_BrowserFetcher_CacheDir"></a>

### CacheDir


```csharp
public string CacheDir { get; set; }
```


Determines the path to download browsers to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L59)

<a id="PuppeteerSharp_BrowserFetcher_Platform_"></a>
<a id="PuppeteerSharp_BrowserFetcher_Platform"></a>

### Platform


```csharp
public Platform Platform { get; set; }
```


Gets the platform.

**Value:** [Platform](/api/PuppeteerSharp/Platform/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L65)

<a id="PuppeteerSharp_BrowserFetcher_WebProxy_"></a>
<a id="PuppeteerSharp_BrowserFetcher_WebProxy"></a>

### WebProxy


```csharp
public IWebProxy WebProxy { get; set; }
```


Proxy used by the HttpClient in [DownloadAsync()](/api/PuppeteerSharp/IBrowserFetcher/#PuppeteerSharp_IBrowserFetcher_DownloadAsync), [DownloadAsync(string)](/api/PuppeteerSharp/IBrowserFetcher/#PuppeteerSharp_IBrowserFetcher_DownloadAsync_System_String_) and [CanDownloadAsync(string)](/api/PuppeteerSharp/IBrowserFetcher/#PuppeteerSharp_IBrowserFetcher_CanDownloadAsync_System_String_).

**Value:** [IWebProxy](https://learn.microsoft.com/dotnet/api/system.net.iwebproxy)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L71)

## Methods

<a id="PuppeteerSharp_BrowserFetcher_CanDownloadAsync_"></a>
<a id="PuppeteerSharp_BrowserFetcher_CanDownloadAsync_System_String_"></a>

### CanDownloadAsync(string)


```csharp
public Task<bool> CanDownloadAsync(string revision)
```


The method initiates a HEAD request to check if the revision is available.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `revision` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Whether the version is available or not.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L74)

<a id="PuppeteerSharp_BrowserFetcher_DownloadAsync_"></a>
<a id="PuppeteerSharp_BrowserFetcher_DownloadAsync"></a>

### DownloadAsync()


```csharp
public Task<InstalledBrowser> DownloadAsync()
```


Downloads the revision.

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L100)

<a id="PuppeteerSharp_BrowserFetcher_DownloadAsync_PuppeteerSharp_BrowserTag_"></a>

### DownloadAsync(BrowserTag)


```csharp
public Task<InstalledBrowser> DownloadAsync(BrowserTag tag)
```


Downloads the revision.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `tag` | [BrowserTag](/api/PuppeteerSharp/BrowserTag/) | Browser tag. |

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L114)

<a id="PuppeteerSharp_BrowserFetcher_DownloadAsync_System_String_"></a>

### DownloadAsync(string)


```csharp
public Task<InstalledBrowser> DownloadAsync(string buildId)
```


Downloads the revision.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |

**Returns:** [Task&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the completed download.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L129)

<a id="PuppeteerSharp_BrowserFetcher_GetExecutablePath_"></a>
<a id="PuppeteerSharp_BrowserFetcher_GetExecutablePath_System_String_"></a>

### GetExecutablePath(string)


```csharp
public string GetExecutablePath(string buildId)
```


Gets the executable path.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Browser buildId. |

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The executable path.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L142)

<a id="PuppeteerSharp_BrowserFetcher_GetInstalledBrowsers_"></a>
<a id="PuppeteerSharp_BrowserFetcher_GetInstalledBrowsers"></a>

### GetInstalledBrowsers()


```csharp
public IEnumerable<InstalledBrowser> GetInstalledBrowsers()
```


A list of all browsers available locally on disk.

**Returns:** [IEnumerable&lt;InstalledBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ienumerable-1) — The available browsers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L121)

<a id="PuppeteerSharp_BrowserFetcher_Uninstall_"></a>
<a id="PuppeteerSharp_BrowserFetcher_Uninstall_System_String_"></a>

### Uninstall(string)


```csharp
public void Uninstall(string buildId)
```


Removes a downloaded browser.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `buildId` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Browser to remove. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/BrowserFetcher.cs#L125)
