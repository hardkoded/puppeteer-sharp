---
title: "JSHandle"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "JSHandle"
  hidden: true
---

<a id="PuppeteerSharp_JSHandle"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class JSHandle : IJSHandle, IAsyncDisposable
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IJSHandle](/api/PuppeteerSharp/IJSHandle/), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable)

**Derived:** [CdpJSHandle](/api/PuppeteerSharp.Cdp/CdpJSHandle/), [ElementHandle](/api/PuppeteerSharp/ElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L10)

## Properties

<a id="PuppeteerSharp_JSHandle_Disposed_"></a>
<a id="PuppeteerSharp_JSHandle_Disposed"></a>

### Disposed


```csharp
public bool Disposed { get; protected set; }
```


Gets a value indicating whether this [IJSHandle](/api/PuppeteerSharp/IJSHandle/) is disposed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if disposed; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L19)

## Methods

<a id="PuppeteerSharp_JSHandle_CheckAndResetMoved_"></a>
<a id="PuppeteerSharp_JSHandle_CheckAndResetMoved"></a>

### CheckAndResetMoved()


```csharp
protected bool CheckAndResetMoved()
```


Checks if the handle has been moved and should skip disposal.
If moved, resets the flag and returns `true`.

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if disposal should be skipped.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L97)

<a id="PuppeteerSharp_JSHandle_DisposeAsync_"></a>
<a id="PuppeteerSharp_JSHandle_DisposeAsync"></a>

### DisposeAsync()


```csharp
public abstract ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L70)

<a id="PuppeteerSharp_JSHandle_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_JSHandle_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
public Task<JsonElement?> EvaluateFunctionAsync(string script, params object[] args)
```


Executes a function in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task&lt;JsonElement?&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L79)

<a id="PuppeteerSharp_JSHandle_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
public Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
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
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L87)

<a id="PuppeteerSharp_JSHandle_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_JSHandle_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
public Task<IJSHandle> EvaluateFunctionHandleAsync(string pageFunction, params object[] args)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Function arguments. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L73)

<a id="PuppeteerSharp_JSHandle_GetPropertiesAsync_"></a>
<a id="PuppeteerSharp_JSHandle_GetPropertiesAsync"></a>

### GetPropertiesAsync()


```csharp
public virtual Task<Dictionary<string, IJSHandle>> GetPropertiesAsync()
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L36)

<a id="PuppeteerSharp_JSHandle_GetPropertyAsync_"></a>
<a id="PuppeteerSharp_JSHandle_GetPropertyAsync_System_String_"></a>

### GetPropertyAsync(string)


```csharp
public virtual Task<IJSHandle> GetPropertyAsync(string propertyName)
```


Fetches a single property from the referenced object.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `propertyName` | [string](https://learn.microsoft.com/dotnet/api/system.string) | property to get. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task of [IJSHandle](/api/PuppeteerSharp/IJSHandle/).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L28)

<a id="PuppeteerSharp_JSHandle_JsonValueAsync_"></a>
<a id="PuppeteerSharp_JSHandle_JsonValueAsync"></a>

### JsonValueAsync()


```csharp
public Task<object> JsonValueAsync()
```


Returns a JSON representation of the object.

**Returns:** [Task&lt;object&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L57)

<a id="PuppeteerSharp_JSHandle_JsonValueAsync__1"></a>

### `JsonValueAsync<T>()`


```csharp
public abstract Task<T> JsonValueAsync<T>()
```


Returns a JSON representation of the object.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | A strongly typed object to parse to. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task.

The method will return an empty JSON if the referenced object is not stringifiable. It will throw an error if the object has circular references.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L60)

<a id="PuppeteerSharp_JSHandle_Move_"></a>
<a id="PuppeteerSharp_JSHandle_Move"></a>

### Move()


```csharp
public IJSHandle Move()
```


Prevents the handle from being disposed on the next [DisposeAsync()](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable.disposeasync) call.
This is useful when you want to keep the handle alive past an `await using` scope.

**Returns:** [IJSHandle](/api/PuppeteerSharp/IJSHandle/) — The handle itself.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/JSHandle.cs#L63)
