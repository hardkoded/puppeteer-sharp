---
title: "WaitForOptions"
description: "Timeout options."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitForOptions"
  hidden: true
---

<a id="PuppeteerSharp_WaitForOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WaitForOptions
```


Timeout options.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Derived:** [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/), [WaitForNetworkIdleOptions](/api/PuppeteerSharp/WaitForNetworkIdleOptions/), [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForOptions.cs#L8)

## Constructors

<a id="PuppeteerSharp_WaitForOptions__ctor_"></a>
<a id="PuppeteerSharp_WaitForOptions__ctor"></a>

### WaitForOptions()


```csharp
public WaitForOptions()
```


Initializes a new instance of the [WaitForOptions](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForOptions.cs#L13)

<a id="PuppeteerSharp_WaitForOptions__ctor_System_Int32_"></a>

### WaitForOptions(int)


```csharp
public WaitForOptions(int timeout)
```


Initializes a new instance of the [WaitForOptions](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForOptions.cs#L21)

## Properties

<a id="PuppeteerSharp_WaitForOptions_CancellationToken_"></a>
<a id="PuppeteerSharp_WaitForOptions_CancellationToken"></a>

### CancellationToken


```csharp
public CancellationToken CancellationToken { get; set; }
```


A [CancellationToken](#PuppeteerSharp_WaitForOptions_CancellationToken) to abort the wait operation.

**Value:** [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

This is the .NET equivalent of the upstream AbortController/AbortSignal pattern.
When the token is cancelled, the wait operation will throw a [OperationCanceledException](https://learn.microsoft.com/dotnet/api/system.operationcanceledexception).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForOptions.cs#L39)

<a id="PuppeteerSharp_WaitForOptions_Timeout_"></a>
<a id="PuppeteerSharp_WaitForOptions_Timeout"></a>

### Timeout


```csharp
public int? Timeout { get; set; }
```


Maximum time to wait for in milliseconds. Defaults to 30000 (30 seconds). Pass 0 to disable timeout.
The default value can be changed by setting the [DefaultTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultTimeout) property.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForOptions.cs#L30)
