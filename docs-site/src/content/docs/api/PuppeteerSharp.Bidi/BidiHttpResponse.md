---
title: "BidiHttpResponse"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "BidiHttpResponse"
  hidden: true
---

<a id="PuppeteerSharp_Bidi_BidiHttpResponse"></a>

Namespace: [PuppeteerSharp.Bidi](/api/PuppeteerSharp.Bidi/)


```csharp
public class BidiHttpResponse : Response<BidiHttpRequest>, IResponse
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → `Response&lt;BidiHttpRequest&gt;`

**Implements:** [IResponse](/api/PuppeteerSharp/IResponse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpResponse.cs#L34)

## Properties

<a id="PuppeteerSharp_Bidi_BidiHttpResponse_FromCache_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpResponse_FromCache"></a>

### FromCache


```csharp
public override bool FromCache { get; }
```


True if the response was served from either the browser's disk cache or memory cache.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpResponse.cs#L107)

## Methods

<a id="PuppeteerSharp_Bidi_BidiHttpResponse_BufferAsync_"></a>
<a id="PuppeteerSharp_Bidi_BidiHttpResponse_BufferAsync"></a>

### BufferAsync()


```csharp
public override ValueTask<byte[]> BufferAsync()
```


Returns a Task which resolves to a buffer with response body.

**Returns:** [ValueTask&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1) — A Task which resolves to a buffer with response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Bidi/BidiHttpResponse.cs#L110)
