---
title: "Mouse"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Mouse"
  hidden: true
---

<a id="PuppeteerSharp_Input_Mouse"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public abstract class Mouse : IMouse, IDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IMouse](/api/PuppeteerSharp.Input/IMouse/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

**Derived:** [CdpMouse](/api/PuppeteerSharp.Cdp/CdpMouse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L11)

## Methods

<a id="PuppeteerSharp_Input_Mouse_ClickAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_ClickAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(decimal, decimal, ClickOptions)


```csharp
public abstract Task ClickAsync(decimal x, decimal y, ClickOptions options = null)
```


Shortcut for `MoveOptions)`, [DownAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_DownAsync_PuppeteerSharp_Input_ClickOptions_) and [UpAsync(ClickOptions)](/api/PuppeteerSharp.Input/IMouse/#PuppeteerSharp_Input_IMouse_UpAsync_PuppeteerSharp_Input_ClickOptions_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse X location to click. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The target mouse Y location to click. |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the click operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L17)

<a id="PuppeteerSharp_Input_Mouse_Dispose_"></a>
<a id="PuppeteerSharp_Input_Mouse_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L47)

<a id="PuppeteerSharp_Input_Mouse_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected abstract void Dispose(bool disposing)
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L54)

<a id="PuppeteerSharp_Input_Mouse_DownAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DownAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### DownAsync(ClickOptions)


```csharp
public abstract Task DownAsync(ClickOptions options = null)
```


Dispatches a `mousedown` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse down operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L20)

<a id="PuppeteerSharp_Input_Mouse_DragAndDropAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DragAndDropAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_System_Int32_"></a>

### DragAndDropAsync(decimal, decimal, decimal, decimal, int)


```csharp
public abstract Task DragAndDropAsync(decimal startX, decimal startY, decimal endX, decimal endY, int delay = 0)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L41)

<a id="PuppeteerSharp_Input_Mouse_DragAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DragAsync_System_Decimal_System_Decimal_System_Decimal_System_Decimal_"></a>

### DragAsync(decimal, decimal, decimal, decimal)


```csharp
public abstract Task<DragData> DragAsync(decimal startX, decimal startY, decimal endX, decimal endY)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L29)

<a id="PuppeteerSharp_Input_Mouse_DragEnterAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DragEnterAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragEnterAsync(decimal, decimal, DragData)


```csharp
public abstract Task DragEnterAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragenter` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L32)

<a id="PuppeteerSharp_Input_Mouse_DragOverAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DragOverAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DragOverAsync(decimal, decimal, DragData)


```csharp
public abstract Task DragOverAsync(decimal x, decimal y, DragData data)
```


Dispatches a `dragover` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L35)

<a id="PuppeteerSharp_Input_Mouse_DropAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_DropAsync_System_Decimal_System_Decimal_PuppeteerSharp_DragData_"></a>

### DropAsync(decimal, decimal, DragData)


```csharp
public abstract Task DropAsync(decimal x, decimal y, DragData data)
```


Dispatches a `drop` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y coordinate. |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L38)

<a id="PuppeteerSharp_Input_Mouse_MoveAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_MoveAsync_System_Decimal_System_Decimal_PuppeteerSharp_Input_MoveOptions_"></a>

### MoveAsync(decimal, decimal, MoveOptions)


```csharp
public abstract Task MoveAsync(decimal x, decimal y, MoveOptions options = null)
```


Dispatches a `mousemove` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse X coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | The destination mouse Y coordinate. |
| `options` | [MoveOptions](/api/PuppeteerSharp.Input/MoveOptions/) | Options to apply to the move operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L14)

<a id="PuppeteerSharp_Input_Mouse_ResetAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_ResetAsync"></a>

### ResetAsync()


```csharp
public abstract Task ResetAsync()
```


Resets the mouse to the default state: No buttons pressed; position at (0,0).

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L44)

<a id="PuppeteerSharp_Input_Mouse_UpAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_UpAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### UpAsync(ClickOptions)


```csharp
public abstract Task UpAsync(ClickOptions options = null)
```


Dispatches a `mouseup` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | Options to apply to the mouse up operation. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L23)

<a id="PuppeteerSharp_Input_Mouse_WheelAsync_"></a>
<a id="PuppeteerSharp_Input_Mouse_WheelAsync_System_Decimal_System_Decimal_"></a>

### WheelAsync(decimal, decimal)


```csharp
public abstract Task WheelAsync(decimal deltaX, decimal deltaY)
```


Dispatches a `wheel` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `deltaX` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta X. |
| `deltaY` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Delta Y. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/Mouse.cs#L26)
