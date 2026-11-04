---
title: "CdpTouchscreen"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpTouchscreen"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpTouchscreen"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpTouchscreen : Touchscreen, ITouchscreen
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Touchscreen](/api/PuppeteerSharp.Input/Touchscreen/)

**Implements:** [ITouchscreen](/api/PuppeteerSharp.Input/ITouchscreen/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTouchscreen.cs#L30)

## Methods

<a id="PuppeteerSharp_Cdp_CdpTouchscreen_TouchStartAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpTouchscreen_TouchStartAsync_System_Decimal_System_Decimal_"></a>

### TouchStartAsync(decimal, decimal)


```csharp
public override Task<ITouchHandle> TouchStartAsync(decimal x, decimal y)
```


Dispatches a `touchstart` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task&lt;ITouchHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves with a handle for the touch that was started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpTouchscreen.cs#L43)
