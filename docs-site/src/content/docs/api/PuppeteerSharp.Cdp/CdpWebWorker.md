---
title: "CdpWebWorker"
description: "The Worker class represents a WebWorker (<a href=\"https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI\">https://developer.mozilla.org/en-US/docs/We..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpWebWorker"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpWebWorker"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpWebWorker : WebWorker
```


The Worker class represents a WebWorker (&lt;a href="https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API"&gt;https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API&lt;/a&gt;).
The events [WorkerCreated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WorkerCreated) and [WorkerDestroyed](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WorkerDestroyed) are emitted on the page object to signal the worker lifecycle.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [WebWorker](/api/PuppeteerSharp/WebWorker/)

**Example**

```csharp
page.WorkerCreated += (sender, e) => Console.WriteLine('Worker created: ' + e.Worker.Url);
page.WorkerDestroyed += (sender, e) => Console.WriteLine('Worker destroyed: ' + e.Worker.Url);
for (var worker of page.Workers)
{
    Console.WriteLine('  ' + worker.Url);
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L35)

## Properties

<a id="PuppeteerSharp_Cdp_CdpWebWorker_Client_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_Client"></a>

### Client


```csharp
public override ICDPSession Client { get; }
```


The CDP session client the WebWorker belongs to.

**Value:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L90)

## Methods

<a id="PuppeteerSharp_Cdp_CdpWebWorker_CloseAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_CloseAsync"></a>

### CloseAsync()


```csharp
public override Task CloseAsync()
```


Closes the worker.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the worker is closed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L100)

<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateExpressionAsync__1_System_String_"></a>

### `EvaluateExpressionAsync<T>(string)`


```csharp
public override Task<T> EvaluateExpressionAsync<T>(string script)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L158)

<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
public override Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L165)

<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
public override Task EvaluateFunctionAsync(string script, params object[] args)
```


Executes a function in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to script. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.
[IJSHandle](/api/PuppeteerSharp/IJSHandle/) instances can be passed as arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L151)

<a id="PuppeteerSharp_Cdp_CdpWebWorker_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
public override Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
```


Executes a function in the context.

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L144)

<a id="PuppeteerSharp_Cdp_CdpWebWorker_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpWebWorker_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
public override Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
```


Waits for the provided function, &lt;code class="paramref"&gt;script&lt;/code&gt;, to return a truthy value when
evaluated in the worker's context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Function to be evaluated in the worker context until it returns a truthy value. |
| `options` | [WaitForFunctionOptions](/api/PuppeteerSharp/WaitForFunctionOptions/) | Options for configuring waiting behavior. |
| `args` | [object\[\]](https://learn.microsoft.com/dotnet/api/system.object) | Arguments to pass to &lt;code class="paramref"&gt;script&lt;/code&gt;. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that resolves to a [IJSHandle](/api/PuppeteerSharp/IJSHandle/) of the truthy value returned by the function.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpWebWorker.cs#L172)
