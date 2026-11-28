---
title: "CdpHttpRequest"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpHttpRequest"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpHttpRequest"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpHttpRequest : Request<CdpHttpResponse>, IRequest
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → `Request&lt;CdpHttpResponse&gt;`

**Implements:** [IRequest](/api/PuppeteerSharp/IRequest/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L17)

## Properties

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_InterceptResolutionState_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_InterceptResolutionState"></a>

### InterceptResolutionState


```csharp
public override InterceptResolutionState InterceptResolutionState { get; }
```


Gets the current intercept resolution state.

**Value:** [InterceptResolutionState](/api/PuppeteerSharp/InterceptResolutionState/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L79)

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_IsInterceptResolutionHandled_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_IsInterceptResolutionHandled"></a>

### IsInterceptResolutionHandled


```csharp
public override bool IsInterceptResolutionHandled { get; protected set; }
```


Gets whether the intercept resolution has already been handled.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L95)

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_Response_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_Response"></a>

### Response


```csharp
public override CdpHttpResponse Response { get; }
```


**Value:** [CdpHttpResponse](/api/PuppeteerSharp.Cdp/CdpHttpResponse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L76)

## Methods

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_CanBeIntercepted_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_CanBeIntercepted"></a>

### CanBeIntercepted()


```csharp
protected override bool CanBeIntercepted()
```


Determines whether this request can be intercepted.

**Returns:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the request can be intercepted; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L331)

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_ContentAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_ContentAsync"></a>

### ContentAsync()


```csharp
public override Task<byte[]> ContentAsync()
```


Fetches the unmodified POST data bytes for the request from the browser.

**Returns:** [Task&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's raw POST data, or `null` when there is none or fetching it fails.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L119)

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_FetchPostDataAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_FetchPostDataAsync"></a>

### FetchPostDataAsync()


```csharp
public override Task<string> FetchPostDataAsync()
```


Fetches the POST data for the request from the browser.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task which resolves to the request's POST data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L112)

<a id="PuppeteerSharp_Cdp_CdpHttpRequest_VerifyInterception_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpRequest_VerifyInterception"></a>

### VerifyInterception()


```csharp
protected override void VerifyInterception()
```


Verifies that request interception is enabled and the request has not been handled yet.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | Thrown when interception is not enabled or request is already handled. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpRequest.cs#L317)
