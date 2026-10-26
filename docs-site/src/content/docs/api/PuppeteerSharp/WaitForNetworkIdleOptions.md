---
title: "WaitForNetworkIdleOptions"
description: "Optional waiting parameters."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitForNetworkIdleOptions"
  hidden: true
---

<a id="PuppeteerSharp_WaitForNetworkIdleOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WaitForNetworkIdleOptions : WaitForOptions
```


Optional waiting parameters.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForNetworkIdleOptions.cs#L7)

## Properties

<a id="PuppeteerSharp_WaitForNetworkIdleOptions_Concurrency_"></a>
<a id="PuppeteerSharp_WaitForNetworkIdleOptions_Concurrency"></a>

### Concurrency


```csharp
public int Concurrency { get; set; }
```


Maximum number of concurrent network connections to allow before considering network idle.
Defaults to 0, meaning network is idle when there are no active connections.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForNetworkIdleOptions.cs#L18)

<a id="PuppeteerSharp_WaitForNetworkIdleOptions_IdleTime_"></a>
<a id="PuppeteerSharp_WaitForNetworkIdleOptions_IdleTime"></a>

### IdleTime


```csharp
public int? IdleTime { get; set; }
```


How long to wait for no network requests in milliseconds, defaults to 500 milliseconds.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForNetworkIdleOptions.cs#L12)
