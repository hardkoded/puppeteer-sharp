---
title: "Touchscreen"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Touchscreen"
  hidden: true
---

<a id="PuppeteerSharp_Input_Touchscreen"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public abstract class Touchscreen : ITouchscreen
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [ITouchscreen](/api/PuppeteerSharp.Input/ITouchscreen/)

**Derived:** [CdpTouchscreen](/api/PuppeteerSharp.Cdp/CdpTouchscreen/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Touchscreen.cs#L7)

## Methods

<a id="PuppeteerSharp_Input_Touchscreen_TapAsync_"></a>
<a id="PuppeteerSharp_Input_Touchscreen_TapAsync_System_Decimal_System_Decimal_"></a>

### TapAsync(decimal, decimal)


```csharp
public Task TapAsync(decimal x, decimal y)
```


Dispatches a `touchstart` and `touchend` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Touchscreen.cs#L17)

<a id="PuppeteerSharp_Input_Touchscreen_TouchEndAsync_"></a>
<a id="PuppeteerSharp_Input_Touchscreen_TouchEndAsync"></a>

### TouchEndAsync()


```csharp
public Task TouchEndAsync()
```


Dispatches a `touchend` event on the first touch that is active.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Touchscreen.cs#L38)

<a id="PuppeteerSharp_Input_Touchscreen_TouchMoveAsync_"></a>
<a id="PuppeteerSharp_Input_Touchscreen_TouchMoveAsync_System_Decimal_System_Decimal_"></a>

### TouchMoveAsync(decimal, decimal)


```csharp
public Task TouchMoveAsync(decimal x, decimal y)
```


Dispatches a `touchmove` event on the first touch that is active.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Touchscreen.cs#L27)

<a id="PuppeteerSharp_Input_Touchscreen_TouchStartAsync_"></a>
<a id="PuppeteerSharp_Input_Touchscreen_TouchStartAsync_System_Decimal_System_Decimal_"></a>

### TouchStartAsync(decimal, decimal)


```csharp
public abstract Task<ITouchHandle> TouchStartAsync(decimal x, decimal y)
```


Dispatches a `touchstart` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task&lt;ITouchHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves with a handle for the touch that was started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Touchscreen.cs#L24)
