---
title: "Request<TResponse>"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Request<TResponse>"
  hidden: true
---

<a id="PuppeteerSharp_Request_1"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Request<TResponse> : IRequest where TResponse : IResponse
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Type parameters**

| Name | Description |
| --- | --- |
| `TResponse` |  |

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IRequest](/api/PuppeteerSharp/IRequest/)

**Derived:** [BidiHttpRequest](/api/PuppeteerSharp.Bidi/BidiHttpRequest/), [CdpHttpRequest](/api/PuppeteerSharp.Cdp/CdpHttpRequest/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L9)

## Properties

<a id="PuppeteerSharp_Request_1_FailureText_"></a>
<a id="PuppeteerSharp_Request_1_FailureText"></a>

### FailureText


```csharp
public string FailureText { get; }
```


Gets the failure.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The failure.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L20)

<a id="PuppeteerSharp_Request_1_Frame_"></a>
<a id="PuppeteerSharp_Request_1_Frame"></a>

### Frame


```csharp
public IFrame Frame { get; }
```


Gets the frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/) — The frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L32)

<a id="PuppeteerSharp_Request_1_HasPostData_"></a>
<a id="PuppeteerSharp_Request_1_HasPostData"></a>

### HasPostData


```csharp
public bool HasPostData { get; }
```


True when the request has POST data. Note that [PostData](/api/PuppeteerSharp/IRequest/#PuppeteerSharp_IRequest_PostData) might still be null when this flag is true
when the data is too long or not readily available in the decoded form.
In that case, use [FetchPostDataAsync()](/api/PuppeteerSharp/IRequest/#PuppeteerSharp_IRequest_FetchPostDataAsync).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L56)

<a id="PuppeteerSharp_Request_1_Headers_"></a>
<a id="PuppeteerSharp_Request_1_Headers"></a>

### Headers


```csharp
public virtual Dictionary<string, string> Headers { get; protected set; }
```


Gets the HTTP headers.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — HTTP headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L44)

<a id="PuppeteerSharp_Request_1_Id_"></a>
<a id="PuppeteerSharp_Request_1_Id"></a>

### Id


```csharp
public string Id { get; }
```


Gets the request identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The request identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L14)

<a id="PuppeteerSharp_Request_1_Initiator_"></a>
<a id="PuppeteerSharp_Request_1_Initiator"></a>

### Initiator


```csharp
public Initiator Initiator { get; }
```


Information about the request initiator.

**Value:** [Initiator](/api/PuppeteerSharp/Initiator/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L53)

<a id="PuppeteerSharp_Request_1_InterceptResolutionState_"></a>
<a id="PuppeteerSharp_Request_1_InterceptResolutionState"></a>

### InterceptResolutionState


```csharp
public abstract InterceptResolutionState InterceptResolutionState { get; }
```


Gets the current intercept resolution state.

**Value:** [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L62)

<a id="PuppeteerSharp_Request_1_InterceptionId_"></a>
<a id="PuppeteerSharp_Request_1_InterceptionId"></a>

### InterceptionId


```csharp
public string InterceptionId { get; }
```


Gets the interception identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The interception identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L17)

<a id="PuppeteerSharp_Request_1_IsInterceptResolutionHandled_"></a>
<a id="PuppeteerSharp_Request_1_IsInterceptResolutionHandled"></a>

### IsInterceptResolutionHandled


```csharp
public abstract bool IsInterceptResolutionHandled { get; protected set; }
```


Gets whether the intercept resolution has already been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L59)

<a id="PuppeteerSharp_Request_1_IsNavigationRequest_"></a>
<a id="PuppeteerSharp_Request_1_IsNavigationRequest"></a>

### IsNavigationRequest


```csharp
public bool IsNavigationRequest { get; }
```


Gets whether this request is driving frame's navigation.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L35)

<a id="PuppeteerSharp_Request_1_Method_"></a>
<a id="PuppeteerSharp_Request_1_Method"></a>

### Method


```csharp
public HttpMethod Method { get; }
```


Gets the HTTP method.

**Value:** [HttpMethod](https://learn.microsoft.com/dotnet/api/system.net.http.httpmethod) — HTTP method.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L38)

<a id="PuppeteerSharp_Request_1_PostData_"></a>
<a id="PuppeteerSharp_Request_1_PostData"></a>

### PostData


```csharp
public string PostData { get; }
```


Gets the post data.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The post data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L41)

<a id="PuppeteerSharp_Request_1_RedirectChain_"></a>
<a id="PuppeteerSharp_Request_1_RedirectChain"></a>

### RedirectChain


```csharp
public IRequest[] RedirectChain { get; }
```


A redirectChain is a chain of requests initiated to fetch a resource.
If there are no redirects and the request was successful, the chain will be empty.
If a server responds with at least a single redirect, then the chain will contain all the requests that were redirected.
redirectChain is shared between all the requests of the same chain.

**Value:** `IRequest\[\]` — The redirect chain.

**Example**

For example, if the website http://example.com has a single redirect to https://example.com, then the chain will contain one request:


```csharp
var response = await page.GoToAsync("http://example.com");
var chain = response.Request.RedirectChain;
Console.WriteLine(chain.Length); // 1
Console.WriteLine(chain[0].Url); // 'http://example.com'
```


If the website https://google.com has no redirects, then the chain will be empty:


```csharp
var response = await page.GoToAsync("https://google.com");
var chain = response.Request.RedirectChain;
Console.WriteLine(chain.Length); // 0
```

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L50)

<a id="PuppeteerSharp_Request_1_ResourceType_"></a>
<a id="PuppeteerSharp_Request_1_ResourceType"></a>

### ResourceType


```csharp
public ResourceType ResourceType { get; }
```


Gets the type of the resource.

**Value:** [ResourceType](/api/PuppeteerSharp/ResourceType/) — The type of the resource.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L29)

<a id="PuppeteerSharp_Request_1_Response_"></a>
<a id="PuppeteerSharp_Request_1_Response"></a>

### Response


```csharp
public virtual TResponse Response { get; }
```


**Value:** `TResponse`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L23)

<a id="PuppeteerSharp_Request_1_Url_"></a>
<a id="PuppeteerSharp_Request_1_Url"></a>

### Url


```csharp
public string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L47)

## Methods

<a id="PuppeteerSharp_Request_1_AbortAsync_"></a>
<a id="PuppeteerSharp_Request_1_AbortAsync_PuppeteerSharp_RequestAbortErrorCode_System_Nullable_System_Int32__"></a>

### AbortAsync(RequestAbortErrorCode, int?)


```csharp
public abstract Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null)
```


Aborts request. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_).
Exception is immediately thrown if the request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `errorCode` | [RequestAbortErrorCode](/api/PuppeteerSharp/RequestAbortErrorCode/) | Optional error code. Defaults to [Failed](/api/PuppeteerSharp/RequestAbortErrorCode/#PuppeteerSharp_RequestAbortErrorCode_Failed). |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L90)

<a id="PuppeteerSharp_Request_1_CanBeIntercepted_"></a>
<a id="PuppeteerSharp_Request_1_CanBeIntercepted"></a>

### CanBeIntercepted()


```csharp
protected abstract bool CanBeIntercepted()
```


Determines whether this request can be intercepted.

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the request can be intercepted; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L122)

<a id="PuppeteerSharp_Request_1_ContentAsync_"></a>
<a id="PuppeteerSharp_Request_1_ContentAsync"></a>

### ContentAsync()


```csharp
public abstract Task<byte[]> ContentAsync()
```


Fetches the unmodified POST data bytes for the request from the browser.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's raw POST data, or `null` when there is none or fetching it fails.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L98)

<a id="PuppeteerSharp_Request_1_ContinueAsync_"></a>
<a id="PuppeteerSharp_Request_1_ContinueAsync_PuppeteerSharp_Payload_System_Nullable_System_Int32__"></a>

### ContinueAsync(Payload, int?)


```csharp
public abstract Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
```


Continues request with optional request overrides. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is immediately thrown if the request interception is not enabled.
If the URL is set it won't perform a redirect. The request will be silently forwarded to the new url. For example, the address bar will show the original url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `payloadOverrides` | [Payload](/api/PuppeteerSharp/Payload/) | Optional request overwrites. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L84)

<a id="PuppeteerSharp_Request_1_FetchPostDataAsync_"></a>
<a id="PuppeteerSharp_Request_1_FetchPostDataAsync"></a>

### FetchPostDataAsync()


```csharp
public abstract Task<string> FetchPostDataAsync()
```


Fetches the POST data for the request from the browser.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's POST data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L95)

<a id="PuppeteerSharp_Request_1_RespondAsync_"></a>
<a id="PuppeteerSharp_Request_1_RespondAsync_PuppeteerSharp_ResponseData_System_Nullable_System_Int32__"></a>

### RespondAsync(ResponseData, int?)


```csharp
public abstract Task RespondAsync(ResponseData response, int? priority = null)
```


Fulfills request with given response. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is thrown if request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `response` | [ResponseData](/api/PuppeteerSharp/ResponseData/) | Response that will fulfill this request. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L87)

<a id="PuppeteerSharp_Request_1_VerifyInterception_"></a>
<a id="PuppeteerSharp_Request_1_VerifyInterception"></a>

### VerifyInterception()


```csharp
protected abstract void VerifyInterception()
```


Verifies that request interception is enabled and the request has not been handled yet.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | Thrown when interception is not enabled or request is already handled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Request.cs#L116)
