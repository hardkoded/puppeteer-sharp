---
title: "IElementHandle"
description: "Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string)."
editUrl: false
prev: false
next: false
sidebar:
  label: "IElementHandle"
  hidden: true
---

<a id="PuppeteerSharp_IElementHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IElementHandle : IJSHandle, IAsyncDisposable
```


Inherits from [IJSHandle](/api/PuppeteerSharp/IJSHandle/). It represents an in-page DOM element.
ElementHandles can be created by [QuerySelectorAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAsync_System_String_) or [QuerySelectorAllAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_).

**Derived:** [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/), [ElementHandle](/api/PuppeteerSharp/ElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L12)

## Properties

<a id="PuppeteerSharp_IElementHandle_Frame_"></a>
<a id="PuppeteerSharp_IElementHandle_Frame"></a>

### Frame


```csharp
IFrame Frame { get; }
```


Parent frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L17)

## Methods

<a id="PuppeteerSharp_IElementHandle_AsLocator_"></a>
<a id="PuppeteerSharp_IElementHandle_AsLocator"></a>

### AsLocator()


```csharp
Locator AsLocator()
```


Converts this [IElementHandle](./) into a [Locator](/api/PuppeteerSharp.Locators/Locator/).

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A [Locator](/api/PuppeteerSharp.Locators/Locator/) backed by this element handle.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L383)

<a id="PuppeteerSharp_IElementHandle_AutofillAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_"></a>

### AutofillAsync(AutofillData)


```csharp
Task AutofillAsync(AutofillData data)
```


If the element is a form input, you can use [AutofillAsync(AutofillData)](#PuppeteerSharp_IElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_)
to test if the form is compatible with the browser's autofill implementation.
Throws an error if the form cannot be autofilled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [AutofillData](/api/PuppeteerSharp/AutofillData/) | The autofill data. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the autofill is complete.

Currently, Puppeteer supports auto-filling credit card information only and
in Chrome in the new headless and headful modes only.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L377)

<a id="PuppeteerSharp_IElementHandle_BackendNodeIdAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_BackendNodeIdAsync"></a>

### BackendNodeIdAsync()


```csharp
Task<int> BackendNodeIdAsync()
```


When connected using Chrome DevTools Protocol, it returns a DOM.BackendNodeId for the element.

**Returns:** [Task&lt;int&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the backend node ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L364)

<a id="PuppeteerSharp_IElementHandle_BoundingBoxAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_BoundingBoxAsync"></a>

### BoundingBoxAsync()


```csharp
Task<BoundingBox> BoundingBoxAsync()
```


This method returns the bounding box of the element (relative to the main frame),
or null if the element is not visible.

**Returns:** [Task&lt;BoundingBox&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The BoundingBox task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L24)

<a id="PuppeteerSharp_IElementHandle_BoxModelAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_BoxModelAsync"></a>

### BoxModelAsync()


```csharp
Task<BoxModel> BoxModelAsync()
```


returns boxes of the element, or `null` if the element is not visible. Box points are sorted clock-wise.

**Returns:** [Task&lt;BoxModel&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task BoxModel task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L30)

<a id="PuppeteerSharp_IElementHandle_ClickAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ClickAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(ClickOptions)


```csharp
Task ClickAsync(ClickOptions options = null)
```


Scrolls element into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to click in the center of the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ClickOptions](/api/PuppeteerSharp.Input/ClickOptions/) | click options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element is successfully clicked.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | if the element is detached from DOM. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L46)

<a id="PuppeteerSharp_IElementHandle_ClickablePointAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ClickablePointAsync_System_Nullable_PuppeteerSharp_Offset__"></a>

### ClickablePointAsync(Offset?)


```csharp
Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
```


Returns the middle point within an element unless a specific offset is provided.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `offset` | `Offset?` | Optional offset. |

**Returns:** [Task&lt;BoxModelPoint&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that resolves to the clickable point.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | When the node is not visible or not an HTMLElement. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L38)

<a id="PuppeteerSharp_IElementHandle_ContentFrameAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ContentFrameAsync"></a>

### ContentFrameAsync()


```csharp
Task<IFrame> ContentFrameAsync()
```


Resolves the frame associated with the element..

**Returns:** [Task&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame associated with the element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L52)

<a id="PuppeteerSharp_IElementHandle_DragAndDropAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_DragAndDropAsync_PuppeteerSharp_IElementHandle_System_Int32_"></a>

### DragAndDropAsync(IElementHandle, int)


```csharp
Task DragAndDropAsync(IElementHandle target, int delay = 0)
```


Performs a drag, dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `target` | [IElementHandle](./) | Target element. |
| `delay` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | If specified, is the time to wait between `dragover` and `drop` in milliseconds. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L60)

<a id="PuppeteerSharp_IElementHandle_DragAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_DragAsync_PuppeteerSharp_IElementHandle_"></a>

### DragAsync(IElementHandle)


```csharp
[Obsolete("Just use DropAsync instead")]
Task<DragData> DragAsync(IElementHandle target)
```


This method creates and captures a dragevent from the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `target` | [IElementHandle](./) | Target Element. |

**Returns:** [Task&lt;DragData&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves when the message was confirmed by the browser with the drag data.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L76)

<a id="PuppeteerSharp_IElementHandle_DragAsync_System_Decimal_System_Decimal_"></a>

### DragAsync(decimal, decimal)


```csharp
[Obsolete("Just use DropAsync instead")]
Task<DragData> DragAsync(decimal x, decimal y)
```


This method creates and captures a dragevent from the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | X coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Y coordinate. |

**Returns:** [Task&lt;DragData&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves when the message was confirmed by the browser with the drag data.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L68)

<a id="PuppeteerSharp_IElementHandle_DragEnterAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_DragEnterAsync_PuppeteerSharp_DragData_"></a>

### DragEnterAsync(DragData)


```csharp
[Obsolete("Don't useDragEnterAsync. `dragenter` will automatically be performed during dragging. ")]
Task DragEnterAsync(DragData data)
```


Dispatches a `dragenter` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L84)

<a id="PuppeteerSharp_IElementHandle_DragOverAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_DragOverAsync_PuppeteerSharp_DragData_"></a>

### DragOverAsync(DragData)


```csharp
[Obsolete("Don't useDragOverAsync. `dragover` will automatically be performed during dragging. ")]
Task DragOverAsync(DragData data)
```


Dispatches a `dragover` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L96)

<a id="PuppeteerSharp_IElementHandle_DropAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_DropAsync_PuppeteerSharp_DragData_"></a>

### DropAsync(DragData)


```csharp
Task DropAsync(DragData data)
```


Performs a dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L108)

<a id="PuppeteerSharp_IElementHandle_DropAsync_PuppeteerSharp_IElementHandle_"></a>

### DropAsync(IElementHandle)


```csharp
Task DropAsync(IElementHandle element)
```


Performs a dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `element` | [IElementHandle](./) | Element to drop. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L115)

<a id="PuppeteerSharp_IElementHandle_FocusAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_FocusAsync"></a>

### FocusAsync()


```csharp
Task FocusAsync()
```


Calls `focus` [https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus](https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus) on the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L121)

<a id="PuppeteerSharp_IElementHandle_HoverAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_HoverAsync"></a>

### HoverAsync()


```csharp
Task HoverAsync()
```


Scrolls element into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to hover over the center of the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element is successfully hovered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L127)

<a id="PuppeteerSharp_IElementHandle_IsHiddenAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_IsHiddenAsync"></a>

### IsHiddenAsync()


```csharp
Task<bool> IsHiddenAsync()
```


Checks if an element is hidden using the same mechanism as `WaitForSelectorOptions)`.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to true if the element is hidden.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L333)

<a id="PuppeteerSharp_IElementHandle_IsIntersectingViewportAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_IsIntersectingViewportAsync_System_Decimal_"></a>

### IsIntersectingViewportAsync(decimal)


```csharp
Task<bool> IsIntersectingViewportAsync(decimal threshold = 0)
```


Evaluates if the element is visible in the current viewport. If an element is an SVG, we check if the svg owner element is in the viewport instead.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `threshold` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | A number between 0 and 1 specifying the fraction of the element's area that must be visible to pass the check. |

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to true if the element is visible in the current viewport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L134)

<a id="PuppeteerSharp_IElementHandle_IsVisibleAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_IsVisibleAsync"></a>

### IsVisibleAsync()


```csharp
Task<bool> IsVisibleAsync()
```


Checks if an element is visible using the same mechanism as `WaitForSelectorOptions)`.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to true if the element is visible.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L327)

<a id="PuppeteerSharp_IElementHandle_PressAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_PressAsync_System_String_PuppeteerSharp_Input_PressOptions_"></a>

### PressAsync(string, PressOptions)


```csharp
Task PressAsync(string key, PressOptions options = null)
```


Focuses the element, and then uses `DownOptions)` and [UpAsync(string)](/api/PuppeteerSharp.Input/IKeyboard/#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. See `KeyDefinitions` for a list of all key names. |
| `options` | [PressOptions](/api/PuppeteerSharp.Input/PressOptions/) | press options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the key is successfully pressed.

If `key` is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also be generated. The [Text](/api/PuppeteerSharp.Input/DownOptions/#PuppeteerSharp_Input_DownOptions_Text) option can be specified to force an input event to be generated.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L145)

<a id="PuppeteerSharp_IElementHandle_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Runs `element.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L152)

<a id="PuppeteerSharp_IElementHandle_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Runs `element.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L160)

<a id="PuppeteerSharp_IElementHandle_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_QuerySelectorAllHandleAsync_System_String_"></a>

### QuerySelectorAllHandleAsync(string)


```csharp
Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
```


A utility function to be used with `Object%5b%5d)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of `document.querySelectorAll` result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L167)

<a id="PuppeteerSharp_IElementHandle_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
Task<IElementHandle> QuerySelectorAsync(string selector)
```


The method runs `element.querySelector` within the page. If no element matches the selector, the return value resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](./) pointing to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L174)

<a id="PuppeteerSharp_IElementHandle_ScreenshotAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ScreenshotAsync_System_String_"></a>

### ScreenshotAsync(string)


```csharp
Task ScreenshotAsync(string file)
```


This method scrolls element into view if needed, and then uses [ScreenshotAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotAsync_System_String_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L184)

<a id="PuppeteerSharp_IElementHandle_ScreenshotAsync_System_String_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotAsync(string, ElementScreenshotOptions)


```csharp
Task ScreenshotAsync(string file, ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L195)

<a id="PuppeteerSharp_IElementHandle_ScreenshotBase64Async_"></a>
<a id="PuppeteerSharp_IElementHandle_ScreenshotBase64Async"></a>

### ScreenshotBase64Async()


```csharp
Task<string> ScreenshotBase64Async()
```


This method scrolls element into view if needed, and then uses [ScreenshotBase64Async(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L202)

<a id="PuppeteerSharp_IElementHandle_ScreenshotBase64Async_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotBase64Async(ElementScreenshotOptions)


```csharp
Task<string> ScreenshotBase64Async(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotBase64Async(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L210)

<a id="PuppeteerSharp_IElementHandle_ScreenshotDataAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ScreenshotDataAsync"></a>

### ScreenshotDataAsync()


```csharp
Task<byte[]> ScreenshotDataAsync()
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L217)

<a id="PuppeteerSharp_IElementHandle_ScreenshotDataAsync_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotDataAsync(ElementScreenshotOptions)


```csharp
Task<byte[]> ScreenshotDataAsync(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L225)

<a id="PuppeteerSharp_IElementHandle_ScreenshotStreamAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ScreenshotStreamAsync"></a>

### ScreenshotStreamAsync()


```csharp
Task<Stream> ScreenshotStreamAsync()
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L232)

<a id="PuppeteerSharp_IElementHandle_ScreenshotStreamAsync_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotStreamAsync(ElementScreenshotOptions)


```csharp
Task<Stream> ScreenshotStreamAsync(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L240)

<a id="PuppeteerSharp_IElementHandle_ScrollIntoViewAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_ScrollIntoViewAsync"></a>

### ScrollIntoViewAsync()


```csharp
Task ScrollIntoViewAsync()
```


Scrolls the element into view using either the automation protocol client or by calling element.scrollIntoView.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L358)

<a id="PuppeteerSharp_IElementHandle_SelectAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_SelectAsync_System_String___"></a>

### SelectAsync(params string[])


```csharp
Task<string[]> SelectAsync(params string[] values)
```


Triggers a `change` and `input` event once all the provided options have been selected.
If there's no `select` element matching `selector`, the method throws an exception.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `values` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Values of options to select. If the `select` has the `multiple` attribute, all values are considered, otherwise only the first one is taken into account. |

**Returns:** [Task&lt;string\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to an array of option values that have been successfully selected.

**Example**

```csharp
await handle.SelectAsync("blue"); // single selection
await handle.SelectAsync("red", "green", "blue"); // multiple selections
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L254)

<a id="PuppeteerSharp_IElementHandle_TapAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_TapAsync"></a>

### TapAsync()


```csharp
Task TapAsync()
```


Scrolls element into view if needed, and then uses `Decimal)` to tap in the center of the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | if the element is detached from DOM. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L261)

<a id="PuppeteerSharp_IElementHandle_TouchEndAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_TouchEndAsync"></a>

### TouchEndAsync()


```csharp
Task TouchEndAsync()
```


/// Dispatches a `touchendt` event.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L352)

<a id="PuppeteerSharp_IElementHandle_TouchMoveAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_TouchMoveAsync_PuppeteerSharp_Input_ITouchHandle_"></a>

### TouchMoveAsync(ITouchHandle)


```csharp
Task TouchMoveAsync(ITouchHandle touch = null)
```


Dispatches a `touchmove` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `touch` | [ITouchHandle](/api/PuppeteerSharp.Input/ITouchHandle/) | Optional touch handle to move. If not provided, the first active touch is moved. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L346)

<a id="PuppeteerSharp_IElementHandle_TouchStartAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_TouchStartAsync"></a>

### TouchStartAsync()


```csharp
Task<ITouchHandle> TouchStartAsync()
```


Dispatches a `touchstart` event.

**Returns:** [Task&lt;ITouchHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves to the [ITouchHandle](/api/PuppeteerSharp.Input/ITouchHandle/) for the started touch.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L339)

<a id="PuppeteerSharp_IElementHandle_TypeAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_TypeAsync_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, TypeOptions)


```csharp
Task TypeAsync(string text, TypeOptions options = null)
```


Focuses the element, and sends a `keydown`, `keypress`/`input`, and `keyup` event for each character in the text.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `text` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A text to type into a focused element. |
| `options` | [TypeOptions](/api/PuppeteerSharp.Input/TypeOptions/) | type options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

To press a special key, like `Control` or `ArrowDown` use `PressOptions)`.

**Example**

```csharp
elementHandle.TypeAsync("#mytextarea", "Hello"); // Types instantly
elementHandle.TypeAsync("#mytextarea", "World", new TypeOptions { Delay = 100 }); // Types slower, like a user
```


An example of typing into a text field and then submitting the form:


```csharp
var elementHandle = await page.GetElementAsync("input");
await elementHandle.TypeAsync("some text");
await elementHandle.PressAsync("Enter");
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L284)

<a id="PuppeteerSharp_IElementHandle_UploadFileAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_UploadFileAsync_System_Boolean_System_String___"></a>

### UploadFileAsync(bool, params string[])


```csharp
Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
```


Uploads files.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `resolveFilePaths` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Set to true to resolve paths using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Sets the value of the file input to these paths. Paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method expects `elementHandle` to point to an `input element` [https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L293)

<a id="PuppeteerSharp_IElementHandle_UploadFileAsync_System_String___"></a>

### UploadFileAsync(params string[])


```csharp
Task UploadFileAsync(params string[] filePaths)
```


Uploads files.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Sets the value of the file input to these paths. Paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method expects `elementHandle` to point to an `input element` [https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L301)

<a id="PuppeteerSharp_IElementHandle_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForSelectorAsync(string, WaitForSelectorOptions)


```csharp
Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
```


Waits for a selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when element specified by selector string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L310)

<a id="PuppeteerSharp_IElementHandle_XPathAsync_"></a>
<a id="PuppeteerSharp_IElementHandle_XPathAsync_System_String_"></a>

### XPathAsync(string)


```csharp
[Obsolete("Use QuerySelectorAsync instead")]
Task<IElementHandle[]> XPathAsync(string expression)
```


Evaluates the XPath expression relative to the elementHandle. If there's no such element, the method will resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `expression` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to evaluate [https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate](https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate). |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [IElementHandle](./).

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IElementHandle.cs#L320)
