---
title: "ITouchHandle"
description: "The TouchHandle interface exposes methods to manipulate touches that have been started."
editUrl: false
prev: false
next: false
sidebar:
  label: "ITouchHandle"
  hidden: true
---

<a id="PuppeteerSharp_Input_ITouchHandle"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public interface ITouchHandle
```


The TouchHandle interface exposes methods to manipulate touches that have been started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchHandle.cs#L30)

## Methods

<a id="PuppeteerSharp_Input_ITouchHandle_EndAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchHandle_EndAsync"></a>

### EndAsync()


```csharp
Task EndAsync()
```


Dispatches a `touchend` event for this touch.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchHandle.cs#L44)

<a id="PuppeteerSharp_Input_ITouchHandle_MoveAsync_"></a>
<a id="PuppeteerSharp_Input_ITouchHandle_MoveAsync_System_Decimal_System_Decimal_"></a>

### MoveAsync(decimal, decimal)


```csharp
Task MoveAsync(decimal x, decimal y)
```


Dispatches a `touchMove` event for this touch.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Horizontal position of the move. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Vertical position of the move. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ITouchHandle.cs#L38)
