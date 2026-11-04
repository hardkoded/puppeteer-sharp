---
title: "ResponseData"
description: "Response that will fulfill a request."
editUrl: false
prev: false
next: false
sidebar:
  label: "ResponseData"
  hidden: true
---

<a id="PuppeteerSharp_ResponseData"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ResponseData
```


Response that will fulfill a request.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L10)

## Properties

<a id="PuppeteerSharp_ResponseData_Body_"></a>
<a id="PuppeteerSharp_ResponseData_Body"></a>

### Body


```csharp
public string Body { get; set; }
```


Response body (text content).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — Body as text.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L16)

<a id="PuppeteerSharp_ResponseData_BodyData_"></a>
<a id="PuppeteerSharp_ResponseData_BodyData"></a>

### BodyData


```csharp
public byte[] BodyData { get; set; }
```


Response body (binary content).

**Value:** [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte) — The body as binary.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L26)

<a id="PuppeteerSharp_ResponseData_ContentType_"></a>
<a id="PuppeteerSharp_ResponseData_ContentType"></a>

### ContentType


```csharp
public string ContentType { get; set; }
```


If set, equals to setting &lt;a href="https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Type"&gt;https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Content-Type&lt;/a&gt; response header.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The Content-Type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L40)

<a id="PuppeteerSharp_ResponseData_Headers_"></a>
<a id="PuppeteerSharp_ResponseData_Headers"></a>

### Headers


```csharp
public Dictionary<string, object> Headers { get; set; }
```


Response headers. Header values will be converted to strings. Headers with null values will be
ignored. When multiple headers values are required use an [ICollection](https://learn.microsoft.com/dotnet/api/system.collections.icollection)
to add multiple values for the Header key.

**Value:** [Dictionary&lt;string, object&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — Headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L34)

<a id="PuppeteerSharp_ResponseData_Status_"></a>
<a id="PuppeteerSharp_ResponseData_Status"></a>

### Status


```csharp
public HttpStatusCode? Status { get; set; }
```


Response status code.

**Value:** [HttpStatusCode?](https://learn.microsoft.com/dotnet/api/system.net.httpstatuscode) — Status Code.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseData.cs#L46)
