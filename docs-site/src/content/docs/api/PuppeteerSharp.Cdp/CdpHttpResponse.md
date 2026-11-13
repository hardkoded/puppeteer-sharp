---
title: "CdpHttpResponse"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpHttpResponse"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpHttpResponse"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpHttpResponse : Response<CdpHttpRequest>, IResponse
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → `Response&lt;CdpHttpRequest&gt;`

**Implements:** [IResponse](/api/PuppeteerSharp/IResponse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/obj/Release/net10.0/System.Text.RegularExpressions.Generator/System.Text.RegularExpressions.Generator.RegexGenerator/RegexGenerator.g.cs#L7)

## Properties

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_EncodedDataLength_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_EncodedDataLength"></a>

### EncodedDataLength


```csharp
public override long EncodedDataLength { get; }
```


Total number of bytes received for this request over the network.

**Value:** [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value is updated when the response finishes loading. During the `Response` event, it only covers
the bytes received up to the response headers. To get the total encoded transfer size (headers + compressed
body), read it in a `RequestFinished` handler or after awaiting [BufferAsync()](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_BufferAsync).
If the request fails after the response is received, the value is not updated. If the browser does not
report a size, e.g. for `data:` or `file:` URLs, the value is `0`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L62)

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_FromCache_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_FromCache"></a>

### FromCache


```csharp
public override bool FromCache { get; }
```


True if the response was served from either the browser's disk cache or memory cache.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L59)

## Methods

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_BufferAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_BufferAsync"></a>

### BufferAsync()


```csharp
public override ValueTask<byte[]> BufferAsync()
```


Returns a Task which resolves to a buffer with response body.

**Returns:** [ValueTask&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1) — A Task which resolves to a buffer with response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L70)

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_StreamAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_StreamAsync"></a>

### StreamAsync()


```csharp
public override Task<Stream> StreamAsync()
```


Returns a Task which resolves to a stream of the response body, with chunks delivered as they arrive from the network.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a stream of the response body.

Call this from the [Response](/api/PuppeteerSharp/Page/#PuppeteerSharp_Page_Response) event to receive the body while it is still loading.
If the body has already finished loading, the stream contains the whole body as a single chunk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L99)
