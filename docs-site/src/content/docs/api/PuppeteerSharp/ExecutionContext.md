---
title: "ExecutionContext"
description: "The class represents a context for JavaScript execution. Examples of JavaScript contexts are: Each Frame has a separate IExecutionContext All kind of web wor..."
editUrl: false
prev: false
next: false
sidebar:
  label: "ExecutionContext"
  hidden: true
---

<a id="PuppeteerSharp_ExecutionContext"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public sealed class ExecutionContext : IExecutionContext, IDisposable, IAsyncDisposable, IPuppeteerUtilWrapper
```


The class represents a context for JavaScript execution. Examples of JavaScript contexts are:
Each [Frame](/api/PuppeteerSharp/IExecutionContext/#PuppeteerSharp_IExecutionContext_Frame) has a separate [IExecutionContext](/api/PuppeteerSharp/IExecutionContext/)
All kind of web workers have their own contexts.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IExecutionContext](/api/PuppeteerSharp/IExecutionContext/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [IAsyncDisposable](https://learn.microsoft.com/dotnet/api/system.iasyncdisposable), [IPuppeteerUtilWrapper](/api/PuppeteerSharp/IPuppeteerUtilWrapper/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/obj/Release/net10.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L31)

## Properties

<a id="PuppeteerSharp_ExecutionContext_EvaluationScriptUrl_"></a>
<a id="PuppeteerSharp_ExecutionContext_EvaluationScriptUrl"></a>

### EvaluationScriptUrl


```csharp
public static string EvaluationScriptUrl { get; set; }
```


Gets or sets the URL used to identify evaluation scripts.
This can be changed to avoid bot detection by services that look for known automation markers.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L43)

## Methods

<a id="PuppeteerSharp_ExecutionContext_Dispose_"></a>
<a id="PuppeteerSharp_ExecutionContext_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L103)

<a id="PuppeteerSharp_ExecutionContext_DisposeAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_DisposeAsync"></a>

### DisposeAsync()


```csharp
public ValueTask DisposeAsync()
```


Performs application-defined tasks associated with freeing, releasing, or
resetting unmanaged resources asynchronously.

**Returns:** [ValueTask](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask) — A task that represents the asynchronous dispose operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L87)

<a id="PuppeteerSharp_ExecutionContext_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_EvaluateExpressionAsync_System_String_"></a>

### EvaluateExpressionAsync(string)


```csharp
public Task<JsonElement?> EvaluateExpressionAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;JsonElement?&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L63)

<a id="PuppeteerSharp_ExecutionContext_EvaluateExpressionAsync__1_System_String_"></a>

### `EvaluateExpressionAsync<T>(string)`


```csharp
public Task<T> EvaluateExpressionAsync<T>(string script)
```


Executes a script in browser context.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L67)

<a id="PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
public Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L71)

<a id="PuppeteerSharp_ExecutionContext_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_EvaluateFunctionAsync_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L79)

<a id="PuppeteerSharp_ExecutionContext_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L83)

<a id="PuppeteerSharp_ExecutionContext_EvaluateFunctionHandleAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_EvaluateFunctionHandleAsync_System_String_System_Object___"></a>

### EvaluateFunctionHandleAsync(string, params object[])


```csharp
public Task<IJSHandle> EvaluateFunctionHandleAsync(string script, params object[] args)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) |  |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Function arguments. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L75)

<a id="PuppeteerSharp_ExecutionContext_GetPuppeteerUtilAsync_"></a>
<a id="PuppeteerSharp_ExecutionContext_GetPuppeteerUtilAsync"></a>

### GetPuppeteerUtilAsync()


```csharp
public Task<IJSHandle> GetPuppeteerUtilAsync()
```


Gets the PuppeteerUtil handle.

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The PuppeteerUtil handle.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ExecutionContext.cs#L110)
