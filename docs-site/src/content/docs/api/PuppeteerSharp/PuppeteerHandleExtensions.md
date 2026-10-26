---
title: "PuppeteerHandleExtensions"
description: "IJSHandle and IElementHandle Extensions."
editUrl: false
prev: false
next: false
sidebar:
  label: "PuppeteerHandleExtensions"
  hidden: true
---

<a id="PuppeteerSharp_PuppeteerHandleExtensions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public static class PuppeteerHandleExtensions
```


[IJSHandle](/api/PuppeteerSharp/IJSHandle/) and [IElementHandle](/api/PuppeteerSharp/IElementHandle/) Extensions.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L13)

## Methods

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync_System_Threading_Tasks_Task_PuppeteerSharp_IElementHandle__System_String_System_Object___"></a>

### `EvaluateFunctionAsync(Task<IElementHandle>, string, params object[])`


```csharp
public static Task EvaluateFunctionAsync(this Task<IElementHandle> elementHandleTask, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome of &lt;code class="paramref"&gt;elementHandleTask&lt;/code&gt; as the first argument.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `elementHandleTask` | [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | A task that returns an [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If &lt;code class="paramref"&gt;elementHandleTask&lt;/code&gt; resolves to `null`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L23)

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync_System_Threading_Tasks_Task_PuppeteerSharp_IJSHandle__System_String_System_Object___"></a>

### `EvaluateFunctionAsync(Task<IJSHandle>, string, params object[])`


```csharp
public static Task EvaluateFunctionAsync(this Task<IJSHandle> arrayHandleTask, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome of &lt;code class="paramref"&gt;arrayHandleTask&lt;/code&gt; as the first argument. Use only after [QuerySelectorAllHandleAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllHandleAsync_System_String_).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `arrayHandleTask` | [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | A task that returns an [IJSHandle](/api/PuppeteerSharp/IJSHandle/) that represents an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L79)

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync__1_PuppeteerSharp_IElementHandle_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(IElementHandle, string, params object[])`


```csharp
public static Task<T> EvaluateFunctionAsync<T>(this IElementHandle elementHandle, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome the &lt;code class="paramref"&gt;elementHandle&lt;/code&gt; as the first argument.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type of the response. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `elementHandle` | [IElementHandle](/api/PuppeteerSharp/IElementHandle/) | An [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the return value of `pageFunction`.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If &lt;code class="paramref"&gt;elementHandle&lt;/code&gt; is `null`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L60)

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync__1_PuppeteerSharp_IJSHandle_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(IJSHandle, string, params object[])`


```csharp
public static Task<T> EvaluateFunctionAsync<T>(this IJSHandle arrayHandle, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome of &lt;code class="paramref"&gt;arrayHandle&lt;/code&gt; as the first argument. Use only after [QuerySelectorAllHandleAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllHandleAsync_System_String_).

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `arrayHandle` | [IJSHandle](/api/PuppeteerSharp/IJSHandle/) | An [IJSHandle](/api/PuppeteerSharp/IJSHandle/) that represents an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the return value of `pageFunction`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L108)

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync__1_System_Threading_Tasks_Task_PuppeteerSharp_IElementHandle__System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(Task<IElementHandle>, string, params object[])`


```csharp
public static Task<T> EvaluateFunctionAsync<T>(this Task<IElementHandle> elementHandleTask, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome of &lt;code class="paramref"&gt;elementHandleTask&lt;/code&gt; as the first argument.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type of the response. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `elementHandleTask` | [Task&lt;IElementHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | A task that returns an [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the return value of `pageFunction`.

**Exceptions**

| Type | Condition |
| --- | --- |
| [SelectorException](/api/PuppeteerSharp/SelectorException/) | If &lt;code class="paramref"&gt;elementHandleTask&lt;/code&gt; resolves to `null`. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L35)

<a id="PuppeteerSharp_PuppeteerHandleExtensions_EvaluateFunctionAsync__1_System_Threading_Tasks_Task_PuppeteerSharp_IJSHandle__System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(Task<IJSHandle>, string, params object[])`


```csharp
public static Task<T> EvaluateFunctionAsync<T>(this Task<IJSHandle> arrayHandleTask, string pageFunction, params object[] args)
```


Runs &lt;code class="paramref"&gt;pageFunction&lt;/code&gt; within the frame and passes it the outcome of &lt;code class="paramref"&gt;arrayHandleTask&lt;/code&gt; as the first argument. Use only after [QuerySelectorAllHandleAsync(string)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_QuerySelectorAllHandleAsync_System_String_).

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type to deserialize the result to. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `arrayHandleTask` | [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) | A task that returns an [IJSHandle](/api/PuppeteerSharp/IJSHandle/) that represents an array of [IElementHandle](/api/PuppeteerSharp/IElementHandle/) that will be used as the first argument in &lt;code class="paramref"&gt;pageFunction&lt;/code&gt;. |
| `pageFunction` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to `pageFunction`. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the return value of `pageFunction`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerHandleExtensions.cs#L90)
