---
title: "LaunchPWAOptions"
description: "Options for LaunchPWAAsync(LaunchPWAOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "LaunchPWAOptions"
  hidden: true
---

<a id="PuppeteerSharp_LaunchPWAOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class LaunchPWAOptions
```


Options for [LaunchPWAAsync(LaunchPWAOptions)](/api/PuppeteerSharp/IBrowser/#PuppeteerSharp_IBrowser_LaunchPWAAsync_PuppeteerSharp_LaunchPWAOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchPWAOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_LaunchPWAOptions_ManifestId_"></a>
<a id="PuppeteerSharp_LaunchPWAOptions_ManifestId"></a>

### ManifestId


```csharp
public string ManifestId { get; set; }
```


Gets or sets the id from the web app's manifest file.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchPWAOptions.cs#L11)

<a id="PuppeteerSharp_LaunchPWAOptions_Timeout_"></a>
<a id="PuppeteerSharp_LaunchPWAOptions_Timeout"></a>

### Timeout


```csharp
public int? Timeout { get; set; }
```


Gets or sets the maximum time in milliseconds to wait for the app's page target to appear.
Defaults to 30 seconds. Pass `0` to disable the timeout.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchPWAOptions.cs#L22)

<a id="PuppeteerSharp_LaunchPWAOptions_Url_"></a>
<a id="PuppeteerSharp_LaunchPWAOptions_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets or sets an optional URL within the app's scope to launch. Defaults to the app's start URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/LaunchPWAOptions.cs#L16)
