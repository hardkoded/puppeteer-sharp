---
title: "NavigationOptions"
description: "Navigation options used by WaitForNavigationAsync(NavigationOptions) and NavigationOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "NavigationOptions"
  hidden: true
---

<a id="PuppeteerSharp_NavigationOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record NavigationOptions : IEquatable<NavigationOptions>
```


Navigation options used by [WaitForNavigationAsync(NavigationOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForNavigationAsync_PuppeteerSharp_NavigationOptions_) and `NavigationOptions)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;NavigationOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

**Derived:** [ReloadOptions](/api/PuppeteerSharp/ReloadOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_NavigationOptions_CancellationToken_"></a>
<a id="PuppeteerSharp_NavigationOptions_CancellationToken"></a>

### CancellationToken


```csharp
public CancellationToken? CancellationToken { get; set; }
```


Optional cancellation token to abort navigation.

**Value:** [CancellationToken?](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L35)

<a id="PuppeteerSharp_NavigationOptions_Referer_"></a>
<a id="PuppeteerSharp_NavigationOptions_Referer"></a>

### Referer


```csharp
public string Referer { get; set; }
```


Referer header value. If provided it will take prefrence over the referer header value set by `String%7d)`.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The referer.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L25)

<a id="PuppeteerSharp_NavigationOptions_ReferrerPolicy_"></a>
<a id="PuppeteerSharp_NavigationOptions_ReferrerPolicy"></a>

### ReferrerPolicy


```csharp
public string ReferrerPolicy { get; set; }
```


Referrer policy. If provided it will take preference over the referer-policy header value set by `String%7d)`.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L30)

<a id="PuppeteerSharp_NavigationOptions_Timeout_"></a>
<a id="PuppeteerSharp_NavigationOptions_Timeout"></a>

### Timeout


```csharp
public int? Timeout { get; set; }
```


Maximum navigation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

The default value can be changed by setting the [DefaultNavigationTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultNavigationTimeout) or [DefaultTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultTimeout) property.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L14)

<a id="PuppeteerSharp_NavigationOptions_WaitUntil_"></a>
<a id="PuppeteerSharp_NavigationOptions_WaitUntil"></a>

### WaitUntil


```csharp
public WaitUntilNavigation[] WaitUntil { get; set; }
```


When to consider navigation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load). Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), navigation is considered to be successful after all events have been fired.

**Value:** `WaitUntilNavigation\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationOptions.cs#L19)
