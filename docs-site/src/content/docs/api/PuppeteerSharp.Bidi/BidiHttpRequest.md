---
title: "BidiHttpRequest"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiHttpRequest"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiHttpRequest"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiHttpRequest : Request<BidiHttpResponse>, IRequest
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → `Request&lt;BidiHttpResponse&gt;`

**Implements:** [IRequest](/api/PuppeteerSharp/IRequest/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L40)

## Properties

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_Headers_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_Headers"></a>

### Headers


```csharp
public override Dictionary<string, string> Headers { get; }
```


Gets the merged headers including extra HTTP headers.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L62)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_InterceptResolutionState_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_InterceptResolutionState"></a>

### InterceptResolutionState


```csharp
public override InterceptResolutionState InterceptResolutionState { get; }
```


Gets the current intercept resolution state.

**Value:** [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L83)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_IsInterceptResolutionHandled_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_IsInterceptResolutionHandled"></a>

### IsInterceptResolutionHandled


```csharp
public override bool IsInterceptResolutionHandled { get; protected set; }
```


Gets whether the intercept resolution has already been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L99)

## Methods

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_AbortAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_AbortAsync_PuppeteerSharp_RequestAbortErrorCode_System_Nullable_System_Int32__"></a>

### AbortAsync(RequestAbortErrorCode, int?)


```csharp
public override Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null)
```


Aborts request. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_).
Exception is immediately thrown if the request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `errorCode` | [RequestAbortErrorCode](/api/PuppeteerSharp/RequestAbortErrorCode/) | Optional error code. Defaults to [Failed](/api/PuppeteerSharp/RequestAbortErrorCode/#PuppeteerSharp_RequestAbortErrorCode_Failed). |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L207)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_CanBeIntercepted_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_CanBeIntercepted"></a>

### CanBeIntercepted()


```csharp
protected override bool CanBeIntercepted()
```


Determines whether this request can be intercepted.

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the request can be intercepted; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L330)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContentAsync"></a>

### ContentAsync()


```csharp
public override Task<byte[]> ContentAsync()
```


Fetches the unmodified POST data bytes for the request from the browser.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's raw POST data, or `null` when there is none or fetching it fails.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L234)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContinueAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContinueAsync_PuppeteerSharp_Payload_System_Nullable_System_Int32__"></a>

### ContinueAsync(Payload, int?)


```csharp
public override Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
```


Continues request with optional request overrides. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is immediately thrown if the request interception is not enabled.
If the URL is set it won't perform a redirect. The request will be silently forwarded to the new url. For example, the address bar will show the original url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `payloadOverrides` | [Payload](/api/PuppeteerSharp/Payload/) | Optional request overwrites. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L131)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_FetchPostDataAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_FetchPostDataAsync"></a>

### FetchPostDataAsync()


```csharp
public override Task<string> FetchPostDataAsync()
```


Fetches the POST data for the request from the browser.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's POST data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L231)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_RespondAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_RespondAsync_PuppeteerSharp_ResponseData_System_Nullable_System_Int32__"></a>

### RespondAsync(ResponseData, int?)


```csharp
public override Task RespondAsync(ResponseData response, int? priority = null)
```


Fulfills request with given response. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is thrown if request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `response` | [ResponseData](/api/PuppeteerSharp/ResponseData/) | Response that will fulfill this request. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L167)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_VerifyInterception_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_VerifyInterception"></a>

### VerifyInterception()


```csharp
protected override void VerifyInterception()
```


Verifies that request interception is enabled and the request has not been handled yet.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | Thrown when interception is not enabled or request is already handled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L316)
