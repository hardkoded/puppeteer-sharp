---
title: "IJSHandle"
description: "IJSHandle represents an in-page JavaScript object. JSHandles can be created with the EvaluateExpressionHandleAsync(string) and IPage.EvaluateFunctionHandleAs..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IJSHandle"
  hidden: true
---

<a id="PuppeteerSharp_IJSHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IJSHandle : IAsyncDisposable
```


IJSHandle represents an in-page JavaScript object. JSHandles can be created with the [EvaluateExpressionHandleAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_EvaluateExpressionHandleAsync_System_String_) and `IPage.EvaluateFunctionHandleAsync()` methods.

**Derived:** [CdpElementHandle](/api/PuppeteerSharp.Cdp/CdpElementHandle/), [CdpJSHandle](/api/PuppeteerSharp.Cdp/CdpJSHandle/), [ElementHandle](/api/PuppeteerSharp/ElementHandle/), [JSHandle](/api/PuppeteerSharp/JSHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L12)

## Properties

<a id="PuppeteerSharp_IJSHandle_Disposed_"></a>
<a id="PuppeteerSharp_IJSHandle_Disposed"></a>

### Disposed


```csharp
bool Disposed { get; }
```


Gets a value indicating whether this [IJSHandle](./) is disposed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if disposed; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L18)

## Methods

<a id="PuppeteerSharp_IJSHandle_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_IJSHandle_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
```


Executes a function in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task&lt;JsonElement?&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](./) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L30)

<a id="PuppeteerSharp_IJSHandle_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
```


Executes a function in browser context.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](./) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L43)

<a id="PuppeteerSharp_IJSHandle_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_IJSHandle_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Function arguments. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](./) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L55)

<a id="PuppeteerSharp_IJSHandle_GetPropertiesAsync_"></a>
<a id="PuppeteerSharp_IJSHandle_GetPropertiesAsync"></a>

### GetPropertiesAsync()


```csharp
Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
```


Returns a `Dictionary<T>` with property names as keys and [IJSHandle](./) instances for the property values.

**Returns:** [Task&lt;Dictionary&lt;string, IJSHandle&gt;&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to a `Dictionary<T>`.

**Example**

```csharp
var handle = await page.EvaluateExpressionHandle("({window, document})");
var properties = await handle.GetPropertiesAsync();
var windowHandle = properties["window"];
var documentHandle = properties["document"];
await handle.DisposeAsync();
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L70)

<a id="PuppeteerSharp_IJSHandle_GetPropertyAsync_"></a>
<a id="PuppeteerSharp_IJSHandle_GetPropertyAsync_System_String_"></a>

### GetPropertyAsync(string)


```csharp
Task<IJSHandle> GetPropertyAsync(string propertyName)
```


Fetches a single property from the referenced object.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `propertyName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | property to get. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task of [IJSHandle](./).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L77)

<a id="PuppeteerSharp_IJSHandle_JsonValueAsync_"></a>
<a id="PuppeteerSharp_IJSHandle_JsonValueAsync"></a>

### JsonValueAsync()


```csharp
Task<object> JsonValueAsync()
```


Returns a JSON representation of the object.

**Returns:** [Task&lt;object&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L86)

<a id="PuppeteerSharp_IJSHandle_JsonValueAsync__1"></a>

### `JsonValueAsync<T>()`


```csharp
Task<T> JsonValueAsync<T>()
```


Returns a JSON representation of the object.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | A strongly typed object to parse to. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L96)

<a id="PuppeteerSharp_IJSHandle_Move_"></a>
<a id="PuppeteerSharp_IJSHandle_Move"></a>

### Move()


```csharp
IJSHandle Move()
```


Prevents the handle from being disposed on the next [DisposeAsync()](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable.disposeasync) call.
This is useful when you want to keep the handle alive past an `await using` scope.

**Returns:** [IJSHandle](./) — The handle itself.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IJSHandle.cs#L103)
