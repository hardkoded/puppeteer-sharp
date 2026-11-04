---
title: "CdpMouse"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpMouse"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpMouse"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpMouse : Mouse, IMouse, IDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Mouse](/api/PuppeteerSharp.Input/Mouse/)

**Implements:** [IMouse](/api/PuppeteerSharp.Input/IMouse/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L34)

## Methods

<a id="PuppeteerSharp_Cdp_CdpMouse_ClickAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_ClickAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(decimal, decimal, ClickOptions)


```csharp
public override Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
```


Shortcut for `MoveOptions)`, [DownAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_) and [UpAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse X location to click. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse Y location to click. |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the click operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L89)

<a id="PuppeteerSharp_Cdp_CdpMouse_Dispose_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected override void Dispose(bool disposing)
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L308)

<a id="PuppeteerSharp_Cdp_CdpMouse_DownAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### DownAsync(ClickOptions)


```csharp
public override Task DownAsync(ClickOptions options = null)
```


Dispatches a `mousedown` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse down operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L115)

<a id="PuppeteerSharp_Cdp_CdpMouse_DragAndDropAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DragAndDropAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_System_Int32_"></a>

### DragAndDropAsync(decimal, decimal, decimal, decimal, int)


```csharp
public override Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L250)

<a id="PuppeteerSharp_Cdp_CdpMouse_DragAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DragAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_"></a>

### DragAsync(decimal, decimal, decimal, decimal)


```csharp
public override Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L186)

<a id="PuppeteerSharp_Cdp_CdpMouse_DragEnterAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DragEnterAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragEnterAsync(decimal, decimal, DragData)


```csharp
public override Task DragEnterAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragenter` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L211)

<a id="PuppeteerSharp_Cdp_CdpMouse_DragOverAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DragOverAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragOverAsync(decimal, decimal, DragData)


```csharp
public override Task DragOverAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragover` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L224)

<a id="PuppeteerSharp_Cdp_CdpMouse_DropAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_DropAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DropAsync(decimal, decimal, DragData)


```csharp
public override Task DropAsync(decimal x, decimal y, DragData data)
```


Dispatches a `drop` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L237)

<a id="PuppeteerSharp_Cdp_CdpMouse_MoveAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_MoveAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_MoveOptions_"></a>

### MoveAsync(decimal, decimal, MoveOptions)


```csharp
public override Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
```


Dispatches a `mousemove` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse X coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse Y coordinate. |
| `options` | [MoveOptions](/api/PuppeteerSharp.Input/MoveOptions/) | Options to apply to the move operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L51)

<a id="PuppeteerSharp_Cdp_CdpMouse_ResetAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_ResetAsync"></a>

### ResetAsync()


```csharp
public override Task ResetAsync()
```


Resets the mouse to the default state: No buttons pressed; position at (0,0).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L270)

<a id="PuppeteerSharp_Cdp_CdpMouse_UpAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### UpAsync(ClickOptions)


```csharp
public override Task UpAsync(ClickOptions options = null)
```


Dispatches a `mouseup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse up operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L141)

<a id="PuppeteerSharp_Cdp_CdpMouse_WheelAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpMouse_WheelAsync_System_Decimal_System_Decimal_"></a>

### WheelAsync(decimal, decimal)


```csharp
public override Task WheelAsync(decimal deltaX, decimal deltaY)
```


Dispatches a `wheel` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `deltaX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta X. |
| `deltaY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta Y. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpMouse.cs#L167)
