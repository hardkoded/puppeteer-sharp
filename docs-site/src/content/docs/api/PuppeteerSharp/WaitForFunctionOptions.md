---
title: "WaitForFunctionOptions"
description: "Optional waiting parameters."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitForFunctionOptions"
  hidden: true
---

<a id="PuppeteerSharp_WaitForFunctionOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WaitForFunctionOptions : WaitForOptions
```


Optional waiting parameters.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionOptions.cs#L9)

## Properties

<a id="PuppeteerSharp_WaitForFunctionOptions_Polling_"></a>
<a id="PuppeteerSharp_WaitForFunctionOptions_Polling"></a>

### Polling


```csharp
public WaitForFunctionPollingOption Polling { get; set; }
```


An interval at which the `pageFunction` is executed. defaults to [Raf](/api/PuppeteerSharp/WaitForFunctionPollingOption/#PuppeteerSharp_WaitForFunctionPollingOption_Raf).

**Value:** [WaitForFunctionPollingOption](/api/PuppeteerSharp/WaitForFunctionPollingOption/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionOptions.cs#L14)

<a id="PuppeteerSharp_WaitForFunctionOptions_PollingInterval_"></a>
<a id="PuppeteerSharp_WaitForFunctionOptions_PollingInterval"></a>

### PollingInterval


```csharp
public int? PollingInterval { get; set; }
```


An interval at which the `pageFunction` is executed. If no value is specified will use [Polling](#PuppeteerSharp_WaitForFunctionOptions_Polling).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForFunctionOptions.cs#L19)
