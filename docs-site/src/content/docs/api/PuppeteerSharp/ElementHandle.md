---
title: "ElementHandle"
description: "Inherits from IJSHandle. It represents an in-page DOM element. ElementHandles can be created by QuerySelectorAsync(string) or QuerySelectorAllAsync(string)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ElementHandle"
  hidden: true
---

<a id="PuppeteerSharp_ElementHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class ElementHandle : JSHandle, IElementHandle, IJSHandle, IAsyncDisposable
```


Inherits from [IJSHandle](/api/PuppeteerSharp/IJSHandle/). It represents an in-page DOM element.
ElementHandles can be created by [QuerySelectorAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAsync_System_String_) or [QuerySelectorAllAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllAsync_System_String_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [JSHandle](/api/PuppeteerSharp/JSHandle/)

**Implements:** [IElementHandle](/api/PuppeteerSharp/IElementHandle/), [IJSHandle](/api/PuppeteerSharp/IJSHandle/), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

**Derived:** [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L16)

## Properties

<a id="PuppeteerSharp_ElementHandle_Page_"></a>
<a id="PuppeteerSharp_ElementHandle_Page"></a>

### Page


```csharp
protected abstract Page Page { get; }
```


Element's page.

**Value:** [Page](/api/PuppeteerSharp/Page/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L38)

## Methods

<a id="PuppeteerSharp_ElementHandle_AsLocator_"></a>
<a id="PuppeteerSharp_ElementHandle_AsLocator"></a>

### AsLocator()


```csharp
public Locator AsLocator()
```


Converts this [IElementHandle](/api/PuppeteerSharp/IElementHandle/) into a [Locator](/api/PuppeteerSharp.Locators/Locator/).

**Returns:** [Locator](/api/PuppeteerSharp.Locators/Locator/) — A [Locator](/api/PuppeteerSharp.Locators/Locator/) backed by this element handle.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L768)

<a id="PuppeteerSharp_ElementHandle_AssertConnectedElementAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_AssertConnectedElementAsync"></a>

### AssertConnectedElementAsync()


```csharp
protected Task AssertConnectedElementAsync()
```


Checks whether the element is still connected to the browser.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) representing the asynchronous operation.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | The exception if the element is not connected. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L790)

<a id="PuppeteerSharp_ElementHandle_AutofillAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_"></a>

### AutofillAsync(AutofillData)


```csharp
public abstract Task AutofillAsync(AutofillData data)
```


If the element is a form input, you can use [AutofillAsync(AutofillData)](/api/PuppeteerSharp/IElementHandle/#PuppeteerSharp_IElementHandle_AutofillAsync_PuppeteerSharp_AutofillData_)
to test if the form is compatible with the browser's autofill implementation.
Throws an error if the form cannot be autofilled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [AutofillData](/api/PuppeteerSharp/AutofillData/) | The autofill data. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when the autofill is complete.

Currently, Puppeteer supports auto-filling credit card information only and
in Chrome in the new headless and headful modes only.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L765)

<a id="PuppeteerSharp_ElementHandle_BackendNodeIdAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_BackendNodeIdAsync"></a>

### BackendNodeIdAsync()


```csharp
public abstract Task<int> BackendNodeIdAsync()
```


When connected using Chrome DevTools Protocol, it returns a DOM.BackendNodeId for the element.

**Returns:** [Task&lt;int&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the backend node ID.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L762)

<a id="PuppeteerSharp_ElementHandle_BindIsolatedHandleAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_BindIsolatedHandleAsync__2_System_Func___1_System_Threading_Tasks_Task___0___"></a>

### `BindIsolatedHandleAsync<TResult, TElementHandle>(Func<TElementHandle, Task<TResult>>)`


```csharp
protected Task<TResult> BindIsolatedHandleAsync<TResult, TElementHandle>(Func<TElementHandle, Task<TResult>> action) where TElementHandle : ElementHandle
```


Executes a function in the isolated context.

**Type parameters**

| Name | Description |
| --- | --- |
| `TResult` | Return type. |
| `TElementHandle` | Element handle type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `action` | [Func&lt;TElementHandle, Task&lt;TResult&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | Function to execute. |

**Returns:** [Task&lt;TResult&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the result of the function.

**Exceptions**

| Type | Condition |
| --- | --- |
| [ArgumentNullException](https://learn.microsoft.com/dotnet/api/system.argumentnullexception) | Thrown if &lt;code class="paramref"&gt;action&lt;/code&gt; is `null`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L817)

<a id="PuppeteerSharp_ElementHandle_BoundingBoxAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_BoundingBoxAsync"></a>

### BoundingBoxAsync()


```csharp
public Task<BoundingBox> BoundingBoxAsync()
```


This method returns the bounding box of the element (relative to the main frame),
or null if the element is not visible.

**Returns:** [Task&lt;BoundingBox&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The BoundingBox task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L306)

<a id="PuppeteerSharp_ElementHandle_BoxModelAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_BoxModelAsync"></a>

### BoxModelAsync()


```csharp
public Task<BoxModel> BoxModelAsync()
```


returns boxes of the element, or `null` if the element is not visible. Box points are sorted clock-wise.

**Returns:** [Task&lt;BoxModel&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task BoxModel task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L343)

<a id="PuppeteerSharp_ElementHandle_ClickAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ClickAsync_PuppeteerSharp_Input_ClickOptions_"></a>

### ClickAsync(ClickOptions)


```csharp
public Task ClickAsync(ClickOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L150)

<a id="PuppeteerSharp_ElementHandle_ClickablePointAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ClickablePointAsync_System_Nullable_PuppeteerSharp_Offset__"></a>

### ClickablePointAsync(Offset?)


```csharp
public Task<BoxModelPoint> ClickablePointAsync(Offset? offset = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L714)

<a id="PuppeteerSharp_ElementHandle_ContentFrameAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ContentFrameAsync"></a>

### ContentFrameAsync()


```csharp
public abstract Task<IFrame> ContentFrameAsync()
```


Resolves the frame associated with the element..

**Returns:** [Task&lt;IFrame&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the frame associated with the element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L459)

<a id="PuppeteerSharp_ElementHandle_DisposeAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DisposeAsync"></a>

### DisposeAsync()


```csharp
public override ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L748)

<a id="PuppeteerSharp_ElementHandle_DragAndDropAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DragAndDropAsync_PuppeteerSharp_IElementHandle_System_Int32_"></a>

### DragAndDropAsync(IElementHandle, int)


```csharp
public Task DragAndDropAsync(IElementHandle target, int delay = 0)
```


Performs a drag, dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `target` | [IElementHandle](/api/PuppeteerSharp/IElementHandle/) | Target element. |
| `delay` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | If specified, is the time to wait between `dragover` and `drop` in milliseconds. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L691)

<a id="PuppeteerSharp_ElementHandle_DragAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DragAsync_PuppeteerSharp_IElementHandle_"></a>

### DragAsync(IElementHandle)


```csharp
public Task<DragData> DragAsync(IElementHandle target)
```


This method creates and captures a dragevent from the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `target` | [IElementHandle](/api/PuppeteerSharp/IElementHandle/) | Target Element. |

**Returns:** [Task&lt;DragData&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves when the message was confirmed by the browser with the drag data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L589)

<a id="PuppeteerSharp_ElementHandle_DragAsync_System_Decimal_System_Decimal_"></a>

### DragAsync(decimal, decimal)


```csharp
public Task<DragData> DragAsync(decimal x, decimal y)
```


This method creates and captures a dragevent from the element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | X coordinate. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Y coordinate. |

**Returns:** [Task&lt;DragData&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves when the message was confirmed by the browser with the drag data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L536)

<a id="PuppeteerSharp_ElementHandle_DragEnterAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DragEnterAsync_PuppeteerSharp_DragData_"></a>

### DragEnterAsync(DragData)


```csharp
public Task DragEnterAsync(DragData data)
```


Dispatches a `dragenter` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L647)

<a id="PuppeteerSharp_ElementHandle_DragOverAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DragOverAsync_PuppeteerSharp_DragData_"></a>

### DragOverAsync(DragData)


```csharp
public Task DragOverAsync(DragData data)
```


Dispatches a `dragover` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L657)

<a id="PuppeteerSharp_ElementHandle_DropAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_DropAsync_PuppeteerSharp_DragData_"></a>

### DropAsync(DragData)


```csharp
public Task DropAsync(DragData data)
```


Performs a dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `data` | [DragData](/api/PuppeteerSharp/DragData/) | Drag data containing items and operations mask. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L667)

<a id="PuppeteerSharp_ElementHandle_DropAsync_PuppeteerSharp_IElementHandle_"></a>

### DropAsync(IElementHandle)


```csharp
public Task DropAsync(IElementHandle target)
```


Performs a dragenter, dragover, and drop in sequence.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `target` | [IElementHandle](/api/PuppeteerSharp/IElementHandle/) |  |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L677)

<a id="PuppeteerSharp_ElementHandle_FocusAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_FocusAsync"></a>

### FocusAsync()


```csharp
public Task FocusAsync()
```


Calls `focus` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus"&gt;https://developer.mozilla.org/en-US/docs/Web/API/HTMLElement/focus&lt;/a&gt; on the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L212)

<a id="PuppeteerSharp_ElementHandle_GetPropertiesAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_GetPropertiesAsync"></a>

### GetPropertiesAsync()


```csharp
public override Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
```


Returns a `Dictionary%602` with property names as keys and [IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances for the property values.

**Returns:** [Task&lt;Dictionary&lt;string, IJSHandle&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a `Dictionary%602`.

**Example**

```csharp
var handle = await page.EvaluateExpressionHandle("({window, document})");
var properties = await handle.GetPropertiesAsync();
var windowHandle = properties["window"];
var documentHandle = properties["document"];
await handle.DisposeAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L740)

<a id="PuppeteerSharp_ElementHandle_GetPropertyAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_GetPropertyAsync_System_String_"></a>

### GetPropertyAsync(string)


```csharp
public override Task<IJSHandle> GetPropertyAsync(string propertyName)
```


Fetches a single property from the referenced object.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `propertyName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | property to get. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task of [IJSHandle](/api/PuppeteerSharp/IJSHandle/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L736)

<a id="PuppeteerSharp_ElementHandle_HoverAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_HoverAsync"></a>

### HoverAsync()


```csharp
public Task HoverAsync()
```


Scrolls element into view if needed, and then uses [Mouse](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Mouse) to hover over the center of the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element is successfully hovered.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L140)

<a id="PuppeteerSharp_ElementHandle_IsHiddenAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_IsHiddenAsync"></a>

### IsHiddenAsync()


```csharp
public Task<bool> IsHiddenAsync()
```


Checks if an element is hidden using the same mechanism as `WaitForSelectorOptions)`.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to true if the element is hidden.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L732)

<a id="PuppeteerSharp_ElementHandle_IsIntersectingViewportAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_IsIntersectingViewportAsync_System_Decimal_"></a>

### IsIntersectingViewportAsync(decimal)


```csharp
public Task<bool> IsIntersectingViewportAsync(decimal threshold)
```


Evaluates if the element is visible in the current viewport. If an element is an SVG, we check if the svg owner element is in the viewport instead.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `threshold` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | A number between 0 and 1 specifying the fraction of the element's area that must be visible to pass the check. |

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task which resolves to true if the element is visible in the current viewport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L462)

<a id="PuppeteerSharp_ElementHandle_IsVisibleAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_IsVisibleAsync"></a>

### IsVisibleAsync()


```csharp
public Task<bool> IsVisibleAsync()
```


Checks if an element is visible using the same mechanism as `WaitForSelectorOptions)`.

**Returns:** [Task&lt;bool&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to true if the element is visible.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L728)

<a id="PuppeteerSharp_ElementHandle_JsonValueAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_JsonValueAsync__1"></a>

### `JsonValueAsync<T>()`


```csharp
public override Task<T> JsonValueAsync<T>()
```


Returns a JSON representation of the object.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | A strongly typed object to parse to. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L744)

<a id="PuppeteerSharp_ElementHandle_PressAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_PressAsync_System_String_PuppeteerSharp_Input_PressOptions_"></a>

### PressAsync(string, PressOptions)


```csharp
public Task PressAsync(string key, PressOptions options = null)
```


Focuses the element, and then uses `DownOptions)` and [UpAsync(string)](/api/PuppeteerSharp.Input/IKeyboard/#PuppeteerSharp_Input_IKeyboard_UpAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `key` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Name of key to press, such as `ArrowLeft`. See `KeyDefinitions` for a list of all key names. |
| `options` | [PressOptions](/api/PuppeteerSharp.Input/PressOptions/) | press options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the key is successfully pressed.

If `key` is a single character and no modifier keys besides `Shift` are being held down, a `keypress`/`input` event will also be generated. The [Text](/api/PuppeteerSharp.Input/DownOptions/#PuppeteerSharp_Input_DownOptions_Text) option can be specified to force an input event to be generated.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L225)

<a id="PuppeteerSharp_ElementHandle_QuerySelectorAllAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_QuerySelectorAllAsync_System_String_"></a>

### QuerySelectorAllAsync(string)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector)
```


Runs `element.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L247)

<a id="PuppeteerSharp_ElementHandle_QuerySelectorAllAsync_System_String_PuppeteerSharp_QueryOptions_"></a>

### QuerySelectorAllAsync(string, QueryOptions)


```csharp
public Task<IElementHandle[]> QuerySelectorAllAsync(string selector, QueryOptions options)
```


Runs `element.querySelectorAll` within the page. If no elements match the selector, the return value resolve to `Empty%60%601`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |
| `options` | [QueryOptions](/api/PuppeteerSharp/QueryOptions/) | Optional query options. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to ElementHandles pointing to the frame elements.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L251)

<a id="PuppeteerSharp_ElementHandle_QuerySelectorAllHandleAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_QuerySelectorAllHandleAsync_System_String_"></a>

### QuerySelectorAllHandleAsync(string)


```csharp
public Task<IJSHandle> QuerySelectorAllHandleAsync(string selector)
```


A utility function to be used with `Object%5b%5d)`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of `document.querySelectorAll` result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L262)

<a id="PuppeteerSharp_ElementHandle_QuerySelectorAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_QuerySelectorAsync_System_String_"></a>

### QuerySelectorAsync(string)


```csharp
public Task<IElementHandle> QuerySelectorAsync(string selector)
```


The method runs `element.querySelector` within the page. If no element matches the selector, the return value resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector to query element for. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to [IElementHandle](/api/PuppeteerSharp/IElementHandle/) pointing to the frame element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L234)

<a id="PuppeteerSharp_ElementHandle_ScreenshotAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ScreenshotAsync_System_String_"></a>

### ScreenshotAsync(string)


```csharp
public Task ScreenshotAsync(string file)
```


This method scrolls element into view if needed, and then uses [ScreenshotAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotAsync_System_String_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L44)

<a id="PuppeteerSharp_ElementHandle_ScreenshotAsync_System_String_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotAsync(string, ElementScreenshotOptions)


```csharp
public Task ScreenshotAsync(string file, ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `file` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The file path to save the image to. The screenshot type will be inferred from file extension. If path is a relative path, then it is resolved relative to current working directory. If no path is provided, the image won't be saved to the disk. |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L47)

<a id="PuppeteerSharp_ElementHandle_ScreenshotBase64Async_"></a>
<a id="PuppeteerSharp_ElementHandle_ScreenshotBase64Async"></a>

### ScreenshotBase64Async()


```csharp
public Task<string> ScreenshotBase64Async()
```


This method scrolls element into view if needed, and then uses [ScreenshotBase64Async(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L90)

<a id="PuppeteerSharp_ElementHandle_ScreenshotBase64Async_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotBase64Async(ElementScreenshotOptions)


```csharp
public Task<string> ScreenshotBase64Async(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotBase64Async(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotBase64Async_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [string](https://learn.microsoft.com/dotnet/api/system.string) containing the image data as base64.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L93)

<a id="PuppeteerSharp_ElementHandle_ScreenshotDataAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ScreenshotDataAsync"></a>

### ScreenshotDataAsync()


```csharp
public Task<byte[]> ScreenshotDataAsync()
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L70)

<a id="PuppeteerSharp_ElementHandle_ScreenshotDataAsync_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotDataAsync(ElementScreenshotOptions)


```csharp
public Task<byte[]> ScreenshotDataAsync(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [byte](https://learn.microsoft.com/dotnet/api/system.byte)[] containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L86)

<a id="PuppeteerSharp_ElementHandle_ScreenshotStreamAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ScreenshotStreamAsync"></a>

### ScreenshotStreamAsync()


```csharp
public Task<Stream> ScreenshotStreamAsync()
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L63)

<a id="PuppeteerSharp_ElementHandle_ScreenshotStreamAsync_PuppeteerSharp_ElementScreenshotOptions_"></a>

### ScreenshotStreamAsync(ElementScreenshotOptions)


```csharp
public Task<Stream> ScreenshotStreamAsync(ElementScreenshotOptions options)
```


This method scrolls element into view if needed, and then uses [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_) to take a screenshot of the element.
If the element is detached from DOM, the method throws an error.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/) | Screenshot options. |

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) containing the image data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L66)

<a id="PuppeteerSharp_ElementHandle_ScrollIntoViewAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_ScrollIntoViewAsync"></a>

### ScrollIntoViewAsync()


```csharp
public virtual Task ScrollIntoViewAsync()
```


Scrolls the element into view using either the automation protocol client or by calling element.scrollIntoView.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L774)

<a id="PuppeteerSharp_ElementHandle_SelectAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_SelectAsync_System_String___"></a>

### SelectAsync(params string[])


```csharp
public Task<string[]> SelectAsync(params string[] values)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L486)

<a id="PuppeteerSharp_ElementHandle_TapAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_TapAsync"></a>

### TapAsync()


```csharp
public Task TapAsync()
```


Scrolls element into view if needed, and then uses `Decimal)` to tap in the center of the element.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves when the element is successfully tapped.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | if the element is detached from DOM. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L166)

<a id="PuppeteerSharp_ElementHandle_ToString_"></a>
<a id="PuppeteerSharp_ElementHandle_ToString"></a>

### ToString()


```csharp
public override string ToString()
```


Returns a string that represents the current object.

**Returns:** [string](https://learn.microsoft.com/dotnet/api/system.string) — A string that represents the current object.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L41)

<a id="PuppeteerSharp_ElementHandle_TouchEndAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_TouchEndAsync"></a>

### TouchEndAsync()


```csharp
public Task TouchEndAsync()
```


/// Dispatches a `touchendt` event.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L203)

<a id="PuppeteerSharp_ElementHandle_TouchMoveAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_TouchMoveAsync_PuppeteerSharp_Input_ITouchHandle_"></a>

### TouchMoveAsync(ITouchHandle)


```csharp
public Task TouchMoveAsync(ITouchHandle touch = null)
```


Dispatches a `touchmove` event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `touch` | [ITouchHandle](/api/PuppeteerSharp.Input/ITouchHandle/) | Optional touch handle to move. If not provided, the first active touch is moved. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that resolves when the message was confirmed by the browser.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L185)

<a id="PuppeteerSharp_ElementHandle_TouchStartAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_TouchStartAsync"></a>

### TouchStartAsync()


```csharp
public Task<ITouchHandle> TouchStartAsync()
```


Dispatches a `touchstart` event.

**Returns:** [Task&lt;ITouchHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task that resolves to the [ITouchHandle](/api/PuppeteerSharp.Input/ITouchHandle/) for the started touch.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L176)

<a id="PuppeteerSharp_ElementHandle_TypeAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_TypeAsync_System_String_PuppeteerSharp_Input_TypeOptions_"></a>

### TypeAsync(string, TypeOptions)


```csharp
public Task TypeAsync(string text, TypeOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L216)

<a id="PuppeteerSharp_ElementHandle_UploadFileAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_UploadFileAsync_System_Boolean_System_String___"></a>

### UploadFileAsync(bool, params string[])


```csharp
public abstract Task UploadFileAsync(bool resolveFilePaths, params string[] filePaths)
```


Uploads files.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `resolveFilePaths` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Set to true to resolve paths using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Sets the value of the file input to these paths. Paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method expects `elementHandle` to point to an `input element` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"&gt;https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L163)

<a id="PuppeteerSharp_ElementHandle_UploadFileAsync_System_String___"></a>

### UploadFileAsync(params string[])


```csharp
public Task UploadFileAsync(params string[] filePaths)
```


Uploads files.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `filePaths` | [string\[\]](https://learn.microsoft.com/dotnet/api/system.string) | Sets the value of the file input to these paths. Paths are resolved using [GetFullPath(string)](https://learn.microsoft.com/dotnet/api/system.io.path.getfullpath#system-io-path-getfullpath(system-string)). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

This method expects `elementHandle` to point to an `input element` &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input"&gt;https://developer.mozilla.org/en-US/docs/Web/HTML/Element/input&lt;/a&gt;.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L160)

<a id="PuppeteerSharp_ElementHandle_WaitForSelectorAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_WaitForSelectorAsync_System_String_PuppeteerSharp_WaitForSelectorOptions_"></a>

### WaitForSelectorAsync(string, WaitForSelectorOptions)


```csharp
public Task<IElementHandle> WaitForSelectorAsync(string selector, WaitForSelectorOptions options = null)
```


Waits for a selector to be added to the DOM.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A selector of an element to wait for. |
| `options` | [WaitForSelectorOptions](/api/PuppeteerSharp/WaitForSelectorOptions/) | Optional waiting parameters. |

**Returns:** [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves when element specified by selector string is added to DOM.
    Resolves to `null` if waiting for `hidden: true` and selector is not found in DOM.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L73)

<a id="PuppeteerSharp_ElementHandle_XPathAsync_"></a>
<a id="PuppeteerSharp_ElementHandle_XPathAsync_System_String_"></a>

### XPathAsync(string)


```csharp
public Task<IElementHandle[]> XPathAsync(string expression)
```


Evaluates the XPath expression relative to the elementHandle. If there's no such element, the method will resolve to `null`.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `expression` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Expression to evaluate &lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Document/evaluate&lt;/a&gt;. |

**Returns:** [Task&lt;IElementHandle\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementHandle.cs#L290)
