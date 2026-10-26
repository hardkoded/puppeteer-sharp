---
title: "Locator"
description: "Locators describe a strategy of locating elements and performing an action on them. If the action fails because the element is not ready for the action, the..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Locator"
  hidden: true
---

<a id="PuppeteerSharp_Locators_Locator"></a>

Namespace: [PuppeteerSharp.Locators](/api/PuppeteerSharp.Locators/)


```csharp
public abstract class Locator
```


Locators describe a strategy of locating elements and performing an action on
them. If the action fails because the element is not ready for the action,
the whole operation is retried. Various preconditions for a successful action
are checked automatically.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L18)

## Properties

<a id="PuppeteerSharp_Locators_Locator_EnsureElementIsInTheViewport_"></a>
<a id="PuppeteerSharp_Locators_Locator_EnsureElementIsInTheViewport"></a>

### EnsureElementIsInTheViewport


```csharp
protected bool EnsureElementIsInTheViewport { get; set; }
```


Gets or sets a value indicating whether to ensure the element is in the viewport.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L45)

<a id="PuppeteerSharp_Locators_Locator_Timeout_"></a>
<a id="PuppeteerSharp_Locators_Locator_Timeout"></a>

### Timeout


```csharp
protected int Timeout { get; set; }
```


Gets or sets the timeout in milliseconds.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L40)

<a id="PuppeteerSharp_Locators_Locator_Visibility_"></a>
<a id="PuppeteerSharp_Locators_Locator_Visibility"></a>

### Visibility


```csharp
protected VisibilityOption? Visibility { get; set; }
```


Gets or sets the visibility option.

**Value:** `VisibilityOption?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L35)

<a id="PuppeteerSharp_Locators_Locator_WaitForEnabled_"></a>
<a id="PuppeteerSharp_Locators_Locator_WaitForEnabled"></a>

### WaitForEnabled


```csharp
protected bool WaitForEnabled { get; set; }
```


Gets or sets a value indicating whether to wait for the element to be enabled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L54)

<a id="PuppeteerSharp_Locators_Locator_WaitForStableBoundingBox_"></a>
<a id="PuppeteerSharp_Locators_Locator_WaitForStableBoundingBox"></a>

### WaitForStableBoundingBox


```csharp
protected bool WaitForStableBoundingBox { get; set; }
```


Gets or sets a value indicating whether to wait for a stable bounding box.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L63)

## Methods

<a id="PuppeteerSharp_Locators_Locator_ClickAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_ClickAsync_PuppeteerSharp_Locators_LocatorClickOptions_"></a>

### ClickAsync(LocatorClickOptions)


```csharp
public Task ClickAsync(LocatorClickOptions options = null)
```


Clicks the located element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorClickOptions](/api/PuppeteerSharp.Locators/LocatorClickOptions/) | Optional click options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the click is performed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L234)

<a id="PuppeteerSharp_Locators_Locator_FillAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_FillAsync_System_String_PuppeteerSharp_Locators_LocatorActionOptions_"></a>

### FillAsync(string, LocatorActionOptions)


```csharp
public Task FillAsync(string value, LocatorActionOptions options = null)
```


Fills the located element with the provided value.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The value to fill. |
| `options` | [LocatorActionOptions](/api/PuppeteerSharp.Locators/LocatorActionOptions/) | Optional action options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the fill is performed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L293)

<a id="PuppeteerSharp_Locators_Locator_Filter_"></a>
<a id="PuppeteerSharp_Locators_Locator_Filter_System_String_"></a>

### Filter(string)


```csharp
public Locator Filter(string predicate)
```


Creates a new locator that filters elements using the provided predicate.
If the predicate does not match, the locator will retry.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A JavaScript function expression that takes an element and returns a boolean. |

**Returns:** [Locator](./) — A new locator that filters based on the predicate.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L140)

<a id="PuppeteerSharp_Locators_Locator_FilterHandle_"></a>
<a id="PuppeteerSharp_Locators_Locator_FilterHandle_System_Func_PuppeteerSharp_IJSHandle_System_Threading_Tasks_Task_System_Boolean___"></a>

### `FilterHandle(Func<IJSHandle, Task<bool>>)`


```csharp
public Locator FilterHandle(Func<IJSHandle, Task<bool>> predicate)
```


Creates a new locator that filters elements using a handle-based predicate.
If the predicate does not match, the locator will retry.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `predicate` | [Func&lt;IJSHandle, Task&lt;bool&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | A function that takes a handle and returns whether it matches. |

**Returns:** [Locator](./) — A new locator that filters based on the predicate.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L161)

<a id="PuppeteerSharp_Locators_Locator_HoverAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_HoverAsync_PuppeteerSharp_Locators_LocatorActionOptions_"></a>

### HoverAsync(LocatorActionOptions)


```csharp
public Task HoverAsync(LocatorActionOptions options = null)
```


Hovers over the located element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorActionOptions](/api/PuppeteerSharp.Locators/LocatorActionOptions/) | Optional action options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the hover is performed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L257)

<a id="PuppeteerSharp_Locators_Locator_Map_"></a>
<a id="PuppeteerSharp_Locators_Locator_Map_System_String_"></a>

### Map(string)


```csharp
public Locator Map(string mapper)
```


Maps the locator using the provided mapper.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `mapper` | [string](https://learn.microsoft.com/dotnet/api/system.string) | A JavaScript function expression that transforms the element. |

**Returns:** [Locator](./) — A new locator that maps the element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L150)

<a id="PuppeteerSharp_Locators_Locator_MapHandle_"></a>
<a id="PuppeteerSharp_Locators_Locator_MapHandle_System_Func_PuppeteerSharp_IJSHandle_System_Threading_Tasks_Task_PuppeteerSharp_IJSHandle___"></a>

### `MapHandle(Func<IJSHandle, Task<IJSHandle>>)`


```csharp
public Locator MapHandle(Func<IJSHandle, Task<IJSHandle>> mapper)
```


Maps the locator using a handle-based mapper.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `mapper` | [Func&lt;IJSHandle, Task&lt;IJSHandle&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.func-2) | A function that takes a handle and returns a new handle. |

**Returns:** [Locator](./) — A new locator that maps the element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L171)

<a id="PuppeteerSharp_Locators_Locator_Race_"></a>
<a id="PuppeteerSharp_Locators_Locator_Race_PuppeteerSharp_Locators_Locator___"></a>

### Race(params Locator[])


```csharp
public static Locator Race(params Locator[] locators)
```


Creates a new locator that races multiple locators, returning the first one to resolve.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `locators` | `Locator\[\]` | The locators to race. |

**Returns:** [Locator](./) — A new locator that resolves to the first matching locator's result.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L74)

<a id="PuppeteerSharp_Locators_Locator_ScrollAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_ScrollAsync_PuppeteerSharp_Locators_LocatorScrollOptions_"></a>

### ScrollAsync(LocatorScrollOptions)


```csharp
public Task ScrollAsync(LocatorScrollOptions options = null)
```


Scrolls the located element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorScrollOptions](/api/PuppeteerSharp.Locators/LocatorScrollOptions/) | Optional scroll options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the scroll is performed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L270)

<a id="PuppeteerSharp_Locators_Locator_SetEnsureElementIsInTheViewport_"></a>
<a id="PuppeteerSharp_Locators_Locator_SetEnsureElementIsInTheViewport_System_Boolean_"></a>

### SetEnsureElementIsInTheViewport(bool)


```csharp
public Locator SetEnsureElementIsInTheViewport(bool value)
```


Sets whether to ensure the element is scrolled into the viewport.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to ensure the element is in the viewport. |

**Returns:** [Locator](./) — The locator instance for chaining.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L117)

<a id="PuppeteerSharp_Locators_Locator_SetTimeout_"></a>
<a id="PuppeteerSharp_Locators_Locator_SetTimeout_System_Int32_"></a>

### SetTimeout(int)


```csharp
public Locator SetTimeout(int timeout)
```


Sets the timeout for the locator.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Timeout in milliseconds. |

**Returns:** [Locator](./) — The locator instance for chaining.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L84)

<a id="PuppeteerSharp_Locators_Locator_SetVisibility_"></a>
<a id="PuppeteerSharp_Locators_Locator_SetVisibility_System_Nullable_PuppeteerSharp_Locators_VisibilityOption__"></a>

### SetVisibility(VisibilityOption?)


```csharp
public Locator SetVisibility(VisibilityOption? visibility)
```


Sets the visibility option for the locator.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `visibility` | `VisibilityOption?` | The visibility option. |

**Returns:** [Locator](./) — The locator instance for chaining.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L95)

<a id="PuppeteerSharp_Locators_Locator_SetWaitForEnabled_"></a>
<a id="PuppeteerSharp_Locators_Locator_SetWaitForEnabled_System_Boolean_"></a>

### SetWaitForEnabled(bool)


```csharp
public Locator SetWaitForEnabled(bool value)
```


Sets whether to wait for the element to be enabled before acting.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to wait for enabled. |

**Returns:** [Locator](./) — The locator instance for chaining.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L106)

<a id="PuppeteerSharp_Locators_Locator_SetWaitForStableBoundingBox_"></a>
<a id="PuppeteerSharp_Locators_Locator_SetWaitForStableBoundingBox_System_Boolean_"></a>

### SetWaitForStableBoundingBox(bool)


```csharp
public Locator SetWaitForStableBoundingBox(bool value)
```


Sets whether to wait for a stable bounding box before acting.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `value` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether to wait for a stable bounding box. |

**Returns:** [Locator](./) — The locator instance for chaining.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L128)

<a id="PuppeteerSharp_Locators_Locator_WaitAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_WaitAsync_PuppeteerSharp_Locators_LocatorActionOptions_"></a>

### WaitAsync(LocatorActionOptions)


```csharp
public Task WaitAsync(LocatorActionOptions options = null)
```


Waits for the locator to locate an element.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorActionOptions](/api/PuppeteerSharp.Locators/LocatorActionOptions/) | Optional action options. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that completes when the element is located.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L219)

<a id="PuppeteerSharp_Locators_Locator_WaitAsync__1_PuppeteerSharp_Locators_LocatorActionOptions_"></a>

### `WaitAsync<T>(LocatorActionOptions)`


```csharp
public Task<T> WaitAsync<T>(LocatorActionOptions options = null)
```


Waits for the locator to get a value from the page.
Note this requires the value to be JSON-serializable.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type of the return value. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorActionOptions](/api/PuppeteerSharp.Locators/LocatorActionOptions/) | Optional action options. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to the serialized value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L195)

<a id="PuppeteerSharp_Locators_Locator_WaitHandleAsync_"></a>
<a id="PuppeteerSharp_Locators_Locator_WaitHandleAsync_PuppeteerSharp_Locators_LocatorActionOptions_"></a>

### WaitHandleAsync(LocatorActionOptions)


```csharp
public Task<IJSHandle> WaitHandleAsync(LocatorActionOptions options = null)
```


Waits for the locator to get a handle from the page.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [LocatorActionOptions](/api/PuppeteerSharp.Locators/LocatorActionOptions/) | Optional action options. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A task that resolves to a handle for the located element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/Locator.cs#L181)
