---
title: "ITouchscreen"
description: "Provides methods to interact with the touch screen."
editUrl: false
prev: false
next: false
sidebar:
  label: "ITouchscreen"
  hidden: true
---

<a id="PuppeteerSharp_Input_ITouchscreen"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public interface ITouchscreen
```


Provides methods to interact with the touch screen.

**Derived:** [CdpTouchscreen](/api/PuppeteerSharp.Cdp/CdpTouchscreen/), [Touchscreen](/api/PuppeteerSharp.Input/Touchscreen/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchscreen.cs#L8)

## Methods

<a id="PuppeteerSharp_Input_ITouchscreen_TapAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchscreen_TapAsync_System_Decimal_System_Decimal_"></a>

### TapAsync(decimal, decimal)


```csharp
Task TapAsync(decimal x, decimal y)
```


Dispatches a `touchstart` and `touchend` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchscreen.cs#L17)

<a id="PuppeteerSharp_Input_ITouchscreen_TouchEndAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchscreen_TouchEndAsync"></a>

### TouchEndAsync()


```csharp
Task TouchEndAsync()
```


Dispatches a `touchend` event on the first touch that is active.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchscreen.cs#L39)

<a id="PuppeteerSharp_Input_ITouchscreen_TouchMoveAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchscreen_TouchMoveAsync_System_Decimal_System_Decimal_"></a>

### TouchMoveAsync(decimal, decimal)


```csharp
Task TouchMoveAsync(decimal x, decimal y)
```


Dispatches a `touchmove` event on the first touch that is active.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchscreen.cs#L33)

<a id="PuppeteerSharp_Input_ITouchscreen_TouchStartAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchscreen_TouchStartAsync_System_Decimal_System_Decimal_"></a>

### TouchStartAsync(decimal, decimal)


```csharp
Task<ITouchHandle> TouchStartAsync(decimal x, decimal y)
```


Dispatches a `touchstart` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch X location. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The touch Y location. |

**Returns:** [Task&lt;ITouchHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves with a handle for the touch that was started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchscreen.cs#L25)
