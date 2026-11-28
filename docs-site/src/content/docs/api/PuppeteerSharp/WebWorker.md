---
title: "WebWorker"
description: "The Worker class represents a WebWorker (https://developer.mozilla.org/en-US/docs/Web/API/WebWorkersAPI). The events WorkerCreated and WorkerDestroyed are em..."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebWorker"
  hidden: true
---

<a id="PuppeteerSharp_WebWorker"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class WebWorker
```


The Worker class represents a WebWorker ([https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API](https://developer.mozilla.org/en-US/docs/Web/API/Web_Workers_API)).
The events [WorkerCreated](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WorkerCreated) and [WorkerDestroyed](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_WorkerDestroyed) are emitted on the page object to signal the worker lifecycle.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Derived:** [CdpWebWorker](/api/PuppeteerSharp.Cdp/CdpWebWorker/)

**Example**

```csharp
page.WorkerCreated += (sender, e) => Console.WriteLine('Worker created: ' + e.Worker.Url);
page.WorkerDestroyed += (sender, e) => Console.WriteLine('Worker destroyed: ' + e.Worker.Url);
for (var worker of page.Workers)
{
    Console.WriteLine('  ' + worker.Url);
}
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L22)

## Properties

<a id="PuppeteerSharp_WebWorker_Client_"></a>
<a id="PuppeteerSharp_WebWorker_Client"></a>

### Client


```csharp
public abstract ICDPSession Client { get; }
```


The CDP session client the WebWorker belongs to.

**Value:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L106)

<a id="PuppeteerSharp_WebWorker_Url_"></a>
<a id="PuppeteerSharp_WebWorker_Url"></a>

### Url


```csharp
public string Url { get; }
```


Gets the Worker URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — Worker URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L101)

## Events

<a id="PuppeteerSharp_WebWorker_Console"></a>

### Console


```csharp
public event EventHandler<ConsoleEventArgs> Console
```


Fired when the worker calls a console API method.

**Value:** [EventHandler&lt;ConsoleEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L32)

<a id="PuppeteerSharp_WebWorker_ExceptionThrown"></a>

### ExceptionThrown


```csharp
public event EventHandler<RemoteExceptionEventArgs> ExceptionThrown
```


Fired when an uncaught exception happens within the worker. Contains a [RemoteException](/api/PuppeteerSharp/RemoteException/).

**Value:** [EventHandler&lt;RemoteExceptionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The [Handle](/api/PuppeteerSharp/RemoteException/#PuppeteerSharp_RemoteException_Handle) of the exception is disposed right after the event has been
dispatched. Call [Move()](/api/PuppeteerSharp/IJSHandle/#PuppeteerSharp_IJSHandle_Move) on it synchronously inside the handler to keep it alive.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L41)

<a id="PuppeteerSharp_WebWorker_Request"></a>

### Request


```csharp
public event EventHandler<RequestEventArgs> Request
```


Emitted when the worker issues a request. Contains a [IRequest](/api/PuppeteerSharp/IRequest/).

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

Only supported with CDP. With WebDriver BiDi these events are never emitted.



For service workers and shared workers, network events are only delivered once a [WebWorker](./)
instance exists, i.e. after [WorkerAsync()](/api/PuppeteerSharp/ITarget/#PuppeteerSharp_ITarget_WorkerAsync) has been called. Requests issued before that,
such as requests made while a service worker starts up, are not reported.




Listeners are bound to this worker instance: if the worker is terminated and restarted (e.g. an idle
extension service worker), a new [WebWorker](./) is created and listeners must be re-attached.




Requests issued by service workers and shared workers cannot be intercepted:
`ContinueAsync`, `AbortAsync` and `RespondAsync` throw.



This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L63)

<a id="PuppeteerSharp_WebWorker_RequestFailed"></a>

### RequestFailed


```csharp
public event EventHandler<RequestEventArgs> RequestFailed
```


Emitted when a request issued by the worker fails. Contains a [IRequest](/api/PuppeteerSharp/IRequest/).

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The same delivery rules as for [Request](#PuppeteerSharp_WebWorker_Request) apply. This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L87)

<a id="PuppeteerSharp_WebWorker_RequestFinished"></a>

### RequestFinished


```csharp
public event EventHandler<RequestEventArgs> RequestFinished
```


Emitted when a request issued by the worker finishes successfully. Contains a [IRequest](/api/PuppeteerSharp/IRequest/).

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The same delivery rules as for [Request](#PuppeteerSharp_WebWorker_Request) apply. This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L95)

<a id="PuppeteerSharp_WebWorker_RequestServedFromCache"></a>

### RequestServedFromCache


```csharp
public event EventHandler<RequestEventArgs> RequestServedFromCache
```


Emitted when a request issued by the worker is served from cache. Contains a [IRequest](/api/PuppeteerSharp/IRequest/).

**Value:** [EventHandler&lt;RequestEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The same delivery rules as for [Request](#PuppeteerSharp_WebWorker_Request) apply. This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L71)

<a id="PuppeteerSharp_WebWorker_Response"></a>

### Response


```csharp
public event EventHandler<ResponseCreatedEventArgs> Response
```


Emitted when a response is received for a request issued by the worker. Contains a [IResponse](/api/PuppeteerSharp/IResponse/).

**Value:** [EventHandler&lt;ResponseCreatedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

The same delivery rules as for [Request](#PuppeteerSharp_WebWorker_Request) apply. This API is experimental.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L79)

## Methods

<a id="PuppeteerSharp_WebWorker_CloseAsync_"></a>
<a id="PuppeteerSharp_WebWorker_CloseAsync"></a>

### CloseAsync()


```csharp
public abstract Task CloseAsync()
```


Closes the worker.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) that completes when the worker is closed.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L206)

<a id="PuppeteerSharp_WebWorker_EvaluateExpressionAsync_"></a>
<a id="PuppeteerSharp_WebWorker_EvaluateExpressionAsync__1_System_String_"></a>

### `EvaluateExpressionAsync<T>(string)`


```csharp
public virtual Task<T> EvaluateExpressionAsync<T>(string script)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L127)

<a id="PuppeteerSharp_WebWorker_EvaluateExpressionHandleAsync_"></a>
<a id="PuppeteerSharp_WebWorker_EvaluateExpressionHandleAsync_System_String_"></a>

### EvaluateExpressionHandleAsync(string)


```csharp
public virtual Task<IJSHandle> EvaluateExpressionHandleAsync(string script)
```


Executes a script in browser context.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `script` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Script to be evaluated in browser context. |

**Returns:** [Task&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to script return value.

If the script, returns a Promise, then the method would wait for the promise to resolve and return its value.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L168)

<a id="PuppeteerSharp_WebWorker_EvaluateFunctionAsync_"></a>
<a id="PuppeteerSharp_WebWorker_EvaluateFunctionAsync_System_String_System_Object___"></a>

### EvaluateFunctionAsync(string, params object[])


```csharp
public virtual Task EvaluateFunctionAsync(string script, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L140)

<a id="PuppeteerSharp_WebWorker_EvaluateFunctionAsync__1_System_String_System_Object___"></a>

### `EvaluateFunctionAsync<T>(string, params object[])`


```csharp
public virtual Task<T> EvaluateFunctionAsync<T>(string script, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L154)

<a id="PuppeteerSharp_WebWorker_WaitForFunctionAsync_"></a>
<a id="PuppeteerSharp_WebWorker_WaitForFunctionAsync_System_String_PuppeteerSharp_WaitForFunctionOptions_System_Object___"></a>

### WaitForFunctionAsync(string, WaitForFunctionOptions, params object[])


```csharp
public virtual Task<IJSHandle> WaitForFunctionAsync(string script, WaitForFunctionOptions options = null, params object[] args)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WebWorker.cs#L179)
