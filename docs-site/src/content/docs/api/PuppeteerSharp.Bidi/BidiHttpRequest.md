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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L56)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_InterceptResolutionState_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_InterceptResolutionState"></a>

### InterceptResolutionState


```csharp
public override InterceptResolutionState InterceptResolutionState { get; }
```


Gets the current intercept resolution state.

**Value:** [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L77)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_IsInterceptResolutionHandled_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_IsInterceptResolutionHandled"></a>

### IsInterceptResolutionHandled


```csharp
public override bool IsInterceptResolutionHandled { get; protected set; }
```


Gets whether the intercept resolution has already been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L93)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_Worker_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_Worker"></a>

### Worker


```csharp
public override WebWorker Worker { get; }
```


Gets the worker that initiated the request, or null if the request was not initiated by a worker.

**Value:** [WebWorker](/api/PuppeteerSharp/WebWorker/) — The worker.

**Exceptions**

| Type | Condition |
| --- | --- |
| [NotSupportedException](https://learn.microsoft.com/dotnet/api/system.notsupportedexception) | Always thrown. Worker requests are not available in WebDriver BiDi. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L97)

## Methods

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_CanBeIntercepted_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_CanBeIntercepted"></a>

### CanBeIntercepted()


```csharp
protected override bool CanBeIntercepted()
```


Determines whether this request can be intercepted.

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the request can be intercepted; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L352)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContentAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_ContentAsync"></a>

### ContentAsync()


```csharp
public override Task<byte[]> ContentAsync()
```


Fetches the unmodified POST data bytes for the request from the browser.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's raw POST data, or `null` when there is none or fetching it fails.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L128)

<a id="PuppeteerSharp_Bidi_BidiHttpRequest_FetchPostDataAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpRequest_FetchPostDataAsync"></a>

### FetchPostDataAsync()


```csharp
public override Task<string> FetchPostDataAsync()
```


Fetches the POST data for the request from the browser.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's POST data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L125)

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpRequest.cs#L338)
