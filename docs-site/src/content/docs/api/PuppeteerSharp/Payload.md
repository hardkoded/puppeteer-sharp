---
title: "Payload"
description: "Payload information."
editUrl: false
prev: false
next: false
sidebar:
  label: "Payload"
  hidden: true
---

<a id="PuppeteerSharp_Payload"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class Payload
```


Payload information.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L9)

## Properties

<a id="PuppeteerSharp_Payload_HasPostData_"></a>
<a id="PuppeteerSharp_Payload_HasPostData"></a>

### HasPostData


```csharp
public bool? HasPostData { get; set; }
```


Gets or sets a value indicating whether this [Payload](./) has post data.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L38)

<a id="PuppeteerSharp_Payload_Headers_"></a>
<a id="PuppeteerSharp_Payload_Headers"></a>

### Headers


```csharp
public Dictionary<string, string> Headers { get; set; }
```


Gets or sets the HTTP headers.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — HTTP headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L27)

<a id="PuppeteerSharp_Payload_Method_"></a>
<a id="PuppeteerSharp_Payload_Method"></a>

### Method


```csharp
public HttpMethod Method { get; set; }
```


Gets or sets the HTTP method.

**Value:** [HttpMethod](https://learn.microsoft.com/dotnet/api/system.net.http.httpmethod) — HTTP method.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L15)

<a id="PuppeteerSharp_Payload_PostData_"></a>
<a id="PuppeteerSharp_Payload_PostData"></a>

### PostData


```csharp
public string PostData { get; set; }
```


Gets or sets the post data.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The post data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L21)

<a id="PuppeteerSharp_Payload_Url_"></a>
<a id="PuppeteerSharp_Payload_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Gets or sets the URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Payload.cs#L33)
