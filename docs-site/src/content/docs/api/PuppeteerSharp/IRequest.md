---
title: "IRequest"
description: "Whenever the page sends a request, the following events are emitted by puppeteer's page: Request emitted when the request is issued by the page. Response emi..."
editUrl: false
prev: false
next: false
sidebar:
  label: "IRequest"
  hidden: true
---

<a id="PuppeteerSharp_IRequest"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IRequest
```


Whenever the page sends a request, the following events are emitted by puppeteer's page:
[Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request) emitted when the request is issued by the page.
[Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) emitted when/if the response is received for the request.
[RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) emitted when the response body is downloaded and the request is complete.

If request fails at some point, then instead of [RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) event (and possibly instead of [Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) event), the [RequestFailed](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFailed) event is emitted.

If request gets a 'redirect' response, the request is successfully finished with the [RequestFinished](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RequestFinished) event, and a new request is issued to a redirected url.

**Derived:** [BidiHttpRequest](/api/PuppeteerSharp.Bidi/BidiHttpRequest/), [CdpHttpRequest](/api/PuppeteerSharp.Cdp/CdpHttpRequest/), [Request&lt;TResponse&gt;](/api/PuppeteerSharp/Request-1/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L17)

## Properties

<a id="PuppeteerSharp_IRequest_FailureText_"></a>
<a id="PuppeteerSharp_IRequest_FailureText"></a>

### FailureText


```csharp
string FailureText { get; }
```


Gets the failure.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The failure.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L29)

<a id="PuppeteerSharp_IRequest_Frame_"></a>
<a id="PuppeteerSharp_IRequest_Frame"></a>

### Frame


```csharp
IFrame Frame { get; }
```


Gets the frame.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/) — The frame.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L53)

<a id="PuppeteerSharp_IRequest_HasPostData_"></a>
<a id="PuppeteerSharp_IRequest_HasPostData"></a>

### HasPostData


```csharp
bool HasPostData { get; }
```


True when the request has POST data. Note that [PostData](#PuppeteerSharp_IRequest_PostData) might still be null when this flag is true
when the data is too long or not readily available in the decoded form.
In that case, use [FetchPostDataAsync()](#PuppeteerSharp_IRequest_FetchPostDataAsync).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L130)

<a id="PuppeteerSharp_IRequest_Headers_"></a>
<a id="PuppeteerSharp_IRequest_Headers"></a>

### Headers


```csharp
Dictionary<string, string> Headers { get; }
```


Gets the HTTP headers.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — HTTP headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L88)

<a id="PuppeteerSharp_IRequest_Id_"></a>
<a id="PuppeteerSharp_IRequest_Id"></a>

### Id


```csharp
string Id { get; }
```


Gets the request identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The request identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L35)

<a id="PuppeteerSharp_IRequest_Initiator_"></a>
<a id="PuppeteerSharp_IRequest_Initiator"></a>

### Initiator


```csharp
Initiator Initiator { get; }
```


Information about the request initiator.

**Value:** [Initiator](/api/PuppeteerSharp/Initiator/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L123)

<a id="PuppeteerSharp_IRequest_InterceptResolutionState_"></a>
<a id="PuppeteerSharp_IRequest_InterceptResolutionState"></a>

### InterceptResolutionState


```csharp
InterceptResolutionState InterceptResolutionState { get; }
```


Gets the current intercept resolution state.

**Value:** [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L140)

<a id="PuppeteerSharp_IRequest_InterceptionId_"></a>
<a id="PuppeteerSharp_IRequest_InterceptionId"></a>

### InterceptionId


```csharp
string InterceptionId { get; }
```


Gets the interception identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The interception identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L41)

<a id="PuppeteerSharp_IRequest_IsInterceptResolutionHandled_"></a>
<a id="PuppeteerSharp_IRequest_IsInterceptResolutionHandled"></a>

### IsInterceptResolutionHandled


```csharp
bool IsInterceptResolutionHandled { get; }
```


Gets whether the intercept resolution has already been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L135)

<a id="PuppeteerSharp_IRequest_IsNavigationRequest_"></a>
<a id="PuppeteerSharp_IRequest_IsNavigationRequest"></a>

### IsNavigationRequest


```csharp
bool IsNavigationRequest { get; }
```


Gets whether this request is driving frame's navigation.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L70)

<a id="PuppeteerSharp_IRequest_Method_"></a>
<a id="PuppeteerSharp_IRequest_Method"></a>

### Method


```csharp
HttpMethod Method { get; }
```


Gets the HTTP method.

**Value:** [HttpMethod](https://learn.microsoft.com/dotnet/api/system.net.http.httpmethod) — HTTP method.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L76)

<a id="PuppeteerSharp_IRequest_Page_"></a>
<a id="PuppeteerSharp_IRequest_Page"></a>

### Page


```csharp
IPage Page { get; }
```


Gets the page that initiated the request, or null if navigating to error pages or the request is coming from a worker.

**Value:** [IPage](/api/PuppeteerSharp/IPage/) — The page.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L59)

<a id="PuppeteerSharp_IRequest_PostData_"></a>
<a id="PuppeteerSharp_IRequest_PostData"></a>

### PostData


```csharp
string PostData { get; }
```


Gets the post data.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The post data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L82)

<a id="PuppeteerSharp_IRequest_RedirectChain_"></a>
<a id="PuppeteerSharp_IRequest_RedirectChain"></a>

### RedirectChain


```csharp
IRequest[] RedirectChain { get; }
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L118)

<a id="PuppeteerSharp_IRequest_ResourceType_"></a>
<a id="PuppeteerSharp_IRequest_ResourceType"></a>

### ResourceType


```csharp
ResourceType ResourceType { get; }
```


Gets the type of the resource.

**Value:** [ResourceType](/api/PuppeteerSharp/ResourceType/) — The type of the resource.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L47)

<a id="PuppeteerSharp_IRequest_Response_"></a>
<a id="PuppeteerSharp_IRequest_Response"></a>

### Response


```csharp
IResponse Response { get; }
```


Response attached to the request.

**Value:** [IResponse](/api/PuppeteerSharp/IResponse/) — The response.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L23)

<a id="PuppeteerSharp_IRequest_Url_"></a>
<a id="PuppeteerSharp_IRequest_Url"></a>

### Url


```csharp
string Url { get; }
```


Gets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L94)

<a id="PuppeteerSharp_IRequest_Worker_"></a>
<a id="PuppeteerSharp_IRequest_Worker"></a>

### Worker


```csharp
WebWorker Worker { get; }
```


Gets the worker that initiated the request, or null if the request was not initiated by a worker.

**Value:** [WebWorker](/api/PuppeteerSharp/WebWorker/) — The worker.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L65)

## Methods

<a id="PuppeteerSharp_IRequest_AbortAsync_"></a>
<a id="PuppeteerSharp_IRequest_AbortAsync_PuppeteerSharp_RequestAbortErrorCode_System_Nullable_System_Int32__"></a>

### AbortAsync(RequestAbortErrorCode, int?)


```csharp
Task AbortAsync(RequestAbortErrorCode errorCode = RequestAbortErrorCode.Failed, int? priority = null)
```


Aborts request. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_).
Exception is immediately thrown if the request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `errorCode` | [RequestAbortErrorCode](/api/PuppeteerSharp/RequestAbortErrorCode/) | Optional error code. Defaults to [Failed](/api/PuppeteerSharp/RequestAbortErrorCode/#PuppeteerSharp_RequestAbortErrorCode_Failed). |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L169)

<a id="PuppeteerSharp_IRequest_ContentAsync_"></a>
<a id="PuppeteerSharp_IRequest_ContentAsync"></a>

### ContentAsync()


```csharp
Task<byte[]> ContentAsync()
```


Fetches the unmodified POST data bytes for the request from the browser.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's raw POST data, or `null` when there is none or fetching it fails.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L184)

<a id="PuppeteerSharp_IRequest_ContinueAsync_"></a>
<a id="PuppeteerSharp_IRequest_ContinueAsync_PuppeteerSharp_Payload_System_Nullable_System_Int32__"></a>

### ContinueAsync(Payload, int?)


```csharp
Task ContinueAsync(Payload payloadOverrides = null, int? priority = null)
```


Continues request with optional request overrides. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is immediately thrown if the request interception is not enabled.
If the URL is set it won't perform a redirect. The request will be silently forwarded to the new url. For example, the address bar will show the original url.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `payloadOverrides` | [Payload](/api/PuppeteerSharp/Payload/) | Optional request overwrites. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L150)

<a id="PuppeteerSharp_IRequest_FetchPostDataAsync_"></a>
<a id="PuppeteerSharp_IRequest_FetchPostDataAsync"></a>

### FetchPostDataAsync()


```csharp
Task<string> FetchPostDataAsync()
```


Fetches the POST data for the request from the browser.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's POST data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L178)

<a id="PuppeteerSharp_IRequest_RespondAsync_"></a>
<a id="PuppeteerSharp_IRequest_RespondAsync_PuppeteerSharp_ResponseData_System_Nullable_System_Int32__"></a>

### RespondAsync(ResponseData, int?)


```csharp
Task RespondAsync(ResponseData response, int? priority = null)
```


Fulfills request with given response. To use this, request interception should be enabled with [SetRequestInterceptionAsync(bool)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetRequestInterceptionAsync_System_Boolean_). Exception is thrown if request interception is not enabled.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `response` | [ResponseData](/api/PuppeteerSharp/ResponseData/) | Response that will fulfill this request. |
| `priority` | [int?](https://learn.microsoft.com/dotnet/api/system.int32) | Optional intercept abort priority. If provided, intercept will be resolved using cooperative handling rules. Otherwise, intercept will be resolved immediately. IMPORTANT: If you set the priority, you will need to attach Request listener using `Task%7d)` instead of [Request](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Request). |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — Task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IRequest.cs#L159)
