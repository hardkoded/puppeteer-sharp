---
title: "SetContentOptions"
description: "Options used by SetContentOptions) and SetContentOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "SetContentOptions"
  hidden: true
---

<a id="PuppeteerSharp_SetContentOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record SetContentOptions : IEquatable<SetContentOptions>
```


Options used by `SetContentOptions)` and
`SetContentOptions)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;SetContentOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

The [Networkidle0](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle0) and [Networkidle2](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle2)
values are not supported for `SetContent` operations and will be ignored. They have never worked
reliably for this operation in upstream Puppeteer. Use
[WaitForNetworkIdleAsync(WaitForNetworkIdleOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WaitForNetworkIdleAsync_PuppeteerSharp_WaitForNetworkIdleOptions_) separately if you need to wait
for network idle after setting content.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SetContentOptions.cs#L16)

## Properties

<a id="PuppeteerSharp_SetContentOptions_CancellationToken_"></a>
<a id="PuppeteerSharp_SetContentOptions_CancellationToken"></a>

### CancellationToken


```csharp
public CancellationToken? CancellationToken { get; set; }
```


Optional cancellation token to abort the operation.

**Value:** [CancellationToken?](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SetContentOptions.cs#L40)

<a id="PuppeteerSharp_SetContentOptions_Timeout_"></a>
<a id="PuppeteerSharp_SetContentOptions_Timeout"></a>

### Timeout


```csharp
public int? Timeout { get; set; }
```


Maximum operation time in milliseconds, defaults to 30 seconds, pass `0` to disable timeout.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

The default value can be changed by setting the [DefaultNavigationTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultNavigationTimeout) or [DefaultTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultTimeout) property.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SetContentOptions.cs#L24)

<a id="PuppeteerSharp_SetContentOptions_WaitUntil_"></a>
<a id="PuppeteerSharp_SetContentOptions_WaitUntil"></a>

### WaitUntil


```csharp
public WaitUntilNavigation[] WaitUntil { get; set; }
```


When to consider the operation succeeded, defaults to [Load](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Load).
Given an array of [WaitUntilNavigation](/api/PuppeteerSharp/WaitUntilNavigation/), the operation is considered to be successful
after all events have been fired.

**Value:** `WaitUntilNavigation\[\]`

[Networkidle0](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle0) and [Networkidle2](/api/PuppeteerSharp/WaitUntilNavigation/#PuppeteerSharp_WaitUntilNavigation_Networkidle2)
are not supported here.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SetContentOptions.cs#L35)
