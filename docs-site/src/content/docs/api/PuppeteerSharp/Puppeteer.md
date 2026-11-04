---
title: "Puppeteer"
description: "Provides a method to launch a Chromium instance."
editUrl: false
prev: false
next: false
sidebar:
  label: "Puppeteer"
  hidden: true
---

<a id="PuppeteerSharp_Puppeteer"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public static class Puppeteer
```


Provides a method to launch a Chromium instance.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Example**

The following is a typical example of using a Puppeteer to drive automation:


```csharp
var browser = await Puppeteer.LaunchAsync(new LaunchOptions());
var page = await browser.NewPageAsync();
await page.GoToAsync("https://www.google.com");
await Browser.CloseAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L22)

## Properties

<a id="PuppeteerSharp_Puppeteer_Devices_"></a>
<a id="PuppeteerSharp_Puppeteer_Devices"></a>

### Devices


```csharp
public static IReadOnlyDictionary<DeviceDescriptorName, DeviceDescriptor> Devices { get; }
```


Returns a list of devices to be used with [EmulateAsync(DeviceDescriptor)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateAsync_PuppeteerSharp_Mobile_DeviceDescriptor_).

**Value:** [IReadOnlyDictionary&lt;DeviceDescriptorName, DeviceDescriptor&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary-2)

**Example**

```csharp
var iPhone = Puppeteer.Devices[DeviceDescriptorName.IPhone6];
using(var page = await browser.NewPageAsync())
{
    await page.EmulateAsync(iPhone);
    await page.goto('https://www.google.com');
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L41)

<a id="PuppeteerSharp_Puppeteer_ExtraJsonSerializerContext_"></a>
<a id="PuppeteerSharp_Puppeteer_ExtraJsonSerializerContext"></a>

### ExtraJsonSerializerContext


```csharp
public static IJsonTypeInfoResolver ExtraJsonSerializerContext { get; set; }
```


Extra [IJsonTypeInfoResolver](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.metadata.ijsontypeinforesolver) used to serialize and deserialize objects on AOT environments.

**Value:** [IJsonTypeInfoResolver](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.metadata.ijsontypeinforesolver)

This property must be set before using any PuppeteerSharp API.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L67)

<a id="PuppeteerSharp_Puppeteer_NetworkConditions_"></a>
<a id="PuppeteerSharp_Puppeteer_NetworkConditions"></a>

### NetworkConditions


```csharp
public static IReadOnlyDictionary<string, NetworkConditions> NetworkConditions { get; }
```


Returns a list of network conditions to be used with [EmulateNetworkConditionsAsync(NetworkConditions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EmulateNetworkConditionsAsync_PuppeteerSharp_NetworkConditions_).
Actual list of conditions can be found in `Conditions`.

**Value:** [IReadOnlyDictionary&lt;string, NetworkConditions&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlydictionary-2)

**Example**

```csharp
var slow3G = Puppeteer.NetworkConditions["Slow 3G"];
using(var page = await browser.NewPageAsync())
{
    await page.EmulateNetworkConditionsAsync(slow3G);
    await page.goto('https://www.google.com');
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L59)

## Methods

<a id="PuppeteerSharp_Puppeteer_ConnectAsync_"></a>
<a id="PuppeteerSharp_Puppeteer_ConnectAsync_PuppeteerSharp_ConnectOptions_Microsoft_Extensions_Logging_ILoggerFactory_"></a>

### ConnectAsync(ConnectOptions, ILoggerFactory)


```csharp
public static Task<IBrowser> ConnectAsync(ConnectOptions options, ILoggerFactory loggerFactory = null)
```


Attaches Puppeteer to an existing Chromium instance. The browser will be closed when the Browser is disposed.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ConnectOptions](/api/PuppeteerSharp/ConnectOptions/) | Options for connecting. |
| `loggerFactory` | [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) | The logger factory. |

**Returns:** [Task&lt;IBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A connected browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L111)

<a id="PuppeteerSharp_Puppeteer_CreateBrowserFetcher_"></a>
<a id="PuppeteerSharp_Puppeteer_CreateBrowserFetcher_PuppeteerSharp_BrowserFetcherOptions_"></a>

### CreateBrowserFetcher(BrowserFetcherOptions)


```csharp
public static IBrowserFetcher CreateBrowserFetcher(BrowserFetcherOptions options)
```


Creates the browser fetcher.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [BrowserFetcherOptions](/api/PuppeteerSharp/BrowserFetcherOptions/) | Options. |

**Returns:** [IBrowserFetcher](/api/PuppeteerSharp/IBrowserFetcher/) — The browser fetcher.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L119)

<a id="PuppeteerSharp_Puppeteer_GetDefaultArgs_"></a>
<a id="PuppeteerSharp_Puppeteer_GetDefaultArgs_PuppeteerSharp_LaunchOptions_"></a>

### GetDefaultArgs(LaunchOptions)


```csharp
public static string[] GetDefaultArgs(LaunchOptions options = null)
```


Returns an array of argument based on the options provided and the platform where the library is running.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options. |

**Returns:** [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) — Chromium arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L79)

<a id="PuppeteerSharp_Puppeteer_LaunchAsync_"></a>
<a id="PuppeteerSharp_Puppeteer_LaunchAsync_PuppeteerSharp_LaunchOptions_Microsoft_Extensions_Logging_ILoggerFactory_"></a>

### LaunchAsync(LaunchOptions, ILoggerFactory)


```csharp
public static Task<IBrowser> LaunchAsync(LaunchOptions options, ILoggerFactory loggerFactory = null)
```


The method launches a browser instance with given arguments. The browser will be closed when the Browser is disposed.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/) | Options for launching Chrome. |
| `loggerFactory` | [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) | The logger factory. |

**Returns:** [Task&lt;IBrowser&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A connected browser.

See &lt;a href="https://www.howtogeek.com/202825/what%E2%80%99s-the-difference-between-chromium-and-chrome/"&gt;this article&lt;/a&gt;
for a description of the differences between Chromium and Chrome.
&lt;a href="https://chromium.googlesource.com/chromium/src/+/lkcr/docs/chromium_browser_vs_google_chrome.md"&gt;This article&lt;/a&gt; describes some differences for Linux users.

Environment Variables
Puppeteer looks for certain &lt;a href="https://en.wikipedia.org/wiki/Environment_variable"&gt;environment variables&lt;/a&gt;() to aid its operations.
- `PUPPETEER_CHROMIUM_REVISION` - specify a certain version of Chromium you'd like Puppeteer to use. See `ILoggerFactory)` on how executable path is inferred.
  **BEWARE**: Puppeteer is only &lt;a href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy"&gt;guaranteed to work&lt;/a&gt; with the bundled Chromium, use at your own risk.
- `PUPPETEER_EXECUTABLE_PATH` - specify an executable path to be used in `ILoggerFactory)`.
  **BEWARE**: Puppeteer is only &lt;a href="https://github.com/GoogleChrome/puppeteer/#q-why-doesnt-puppeteer-vxxx-work-with-chromium-vyyy"&gt;guaranteed to work&lt;/a&gt; with the bundled Chromium, use at your own risk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L102)

<a id="PuppeteerSharp_Puppeteer_SetFollowSymlinks_"></a>
<a id="PuppeteerSharp_Puppeteer_SetFollowSymlinks_System_Boolean_"></a>

### SetFollowSymlinks(bool)


```csharp
public static void SetFollowSymlinks(bool followSymlinks)
```


Defines whether Puppeteer should follow symlinks for file operations.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `followSymlinks` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether Puppeteer should follow symlinks. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Puppeteer.cs#L125)
