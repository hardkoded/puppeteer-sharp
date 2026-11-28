---
title: "IMouse"
description: "Provides methods to interact with the mouse."
editUrl: false
prev: false
next: false
sidebar:
  label: "IMouse"
  hidden: true
---

<a id="PuppeteerSharp_Input_IMouse"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public interface IMouse : IDisposable
```


Provides methods to interact with the mouse.

**Derived:** [CdpMouse](/api/PuppeteerSharp.Cdp/CdpMouse/), [Mouse](/api/PuppeteerSharp.Input/Mouse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L9)

## Methods

<a id="PuppeteerSharp_Input_IMouse_ClickAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_ClickAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(decimal, decimal, ClickOptions)


```csharp
Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
```


Shortcut for `MoveAsync()`, [DownAsync(ClickOptions)](#PuppeteerSharp_Input_IMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_) and [UpAsync(ClickOptions)](#PuppeteerSharp_Input_IMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse X location to click. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse Y location to click. |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the click operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L18)

<a id="PuppeteerSharp_Input_IMouse_DownAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### DownAsync(ClickOptions)


```csharp
Task DownAsync(ClickOptions options = null)
```


Dispatches a `mousedown` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse down operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L25)

<a id="PuppeteerSharp_Input_IMouse_DragAndDropAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DragAndDropAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_System_Int32_"></a>

### DragAndDropAsync(decimal, decimal, decimal, decimal, int)


```csharp
Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0)
```


Performs a drag, dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `startX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Start X coordinate. |
| `startY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Start Y coordinate. |
| `endX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | End X coordinate. |
| `endY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | End Y coordinate. |
| `delay` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | If specified, is the time to wait between `dragover` and `drop` in milliseconds. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L36)

<a id="PuppeteerSharp_Input_IMouse_DragAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DragAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_"></a>

### DragAsync(decimal, decimal, decimal, decimal)


```csharp
Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
```


Dispatches a `drag` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `startX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Start X coordinate. |
| `startY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Start Y coordinate. |
| `endX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | End X coordinate. |
| `endY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | End Y coordinate. |

**Returns:** [Task&lt;DragData&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves when the message was confirmed by the browser with the drag data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L52)

<a id="PuppeteerSharp_Input_IMouse_DragEnterAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DragEnterAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragEnterAsync(decimal, decimal, DragData)


```csharp
Task DragEnterAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragenter` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L61)

<a id="PuppeteerSharp_Input_IMouse_DragOverAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DragOverAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragOverAsync(decimal, decimal, DragData)


```csharp
Task DragOverAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragover` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L70)

<a id="PuppeteerSharp_Input_IMouse_DropAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_DropAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DropAsync(decimal, decimal, DragData)


```csharp
Task DropAsync(decimal x, decimal y, DragData data)
```


Dispatches a `drop` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L79)

<a id="PuppeteerSharp_Input_IMouse_MoveAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_MoveAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_MoveOptions_"></a>

### MoveAsync(decimal, decimal, MoveOptions)


```csharp
Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
```


Dispatches a `mousemove` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse X coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse Y coordinate. |
| `options` | [MoveOptions](/api/PuppeteerSharp.Input/MoveOptions/) | Options to apply to the move operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L88)

<a id="PuppeteerSharp_Input_IMouse_ResetAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_ResetAsync"></a>

### ResetAsync()


```csharp
Task ResetAsync()
```


Resets the mouse to the default state: No buttons pressed; position at (0,0).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L109)

<a id="PuppeteerSharp_Input_IMouse_UpAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### UpAsync(ClickOptions)


```csharp
Task UpAsync(ClickOptions options = null)
```


Dispatches a `mouseup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse up operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L95)

<a id="PuppeteerSharp_Input_IMouse_WheelAsync_"></a>
<a id="PuppeteerSharp_Input_IMouse_WheelAsync_System_Decimal_System_Decimal_"></a>

### WheelAsync(decimal, decimal)


```csharp
Task WheelAsync(decimal deltaX, decimal deltaY)
```


Dispatches a `wheel` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `deltaX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta X. |
| `deltaY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta Y. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/IMouse.cs#L103)
