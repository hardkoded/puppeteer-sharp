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

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_FromCache_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_FromCache"></a>

### FromCache


```csharp
public override bool FromCache { get; }
```


True if the response was served from either the browser's disk cache or memory cache.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L56)

## Methods

<a id="PuppeteerSharp_Cdp_CdpHttpResponse_BufferAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpHttpResponse_BufferAsync"></a>

### BufferAsync()


```csharp
public override ValueTask<byte[]> BufferAsync()
```


Returns a Task which resolves to a buffer with response body.

**Returns:** [ValueTask&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1) — A Task which resolves to a buffer with response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpHttpResponse.cs#L64)
