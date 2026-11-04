---
title: "Response<TRequest>"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Response<TRequest>"
  hidden: true
---

<a id="PuppeteerSharp_Response_1"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class Response<TRequest> : IResponse where TRequest : IRequest
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Type parameters**

| Name | Description |
| --- | --- |
| `TRequest` |  |

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IResponse](/api/PuppeteerSharp/IResponse/)

**Derived:** [BidiHttpResponse](/api/PuppeteerSharp.Bidi/BidiHttpResponse/), [CdpHttpResponse](/api/PuppeteerSharp.Cdp/CdpHttpResponse/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L14)

## Properties

<a id="PuppeteerSharp_Response_1_Frame_"></a>
<a id="PuppeteerSharp_Response_1_Frame"></a>

### Frame


```csharp
public IFrame Frame { get; }
```


A [Frame](/api/PuppeteerSharp/IResponse/#PuppeteerSharp_IResponse_Frame) that initiated this request. Or null if navigating to error pages.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L53)

<a id="PuppeteerSharp_Response_1_FromCache_"></a>
<a id="PuppeteerSharp_Response_1_FromCache"></a>

### FromCache


```csharp
public abstract bool FromCache { get; }
```


True if the response was served from either the browser's disk cache or memory cache.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L44)

<a id="PuppeteerSharp_Response_1_FromServiceWorker_"></a>
<a id="PuppeteerSharp_Response_1_FromServiceWorker"></a>

### FromServiceWorker


```csharp
public bool FromServiceWorker { get; protected init; }
```


Gets a value indicating whether the [IResponse](/api/PuppeteerSharp/IResponse/) was served by a service worker.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the [IResponse](/api/PuppeteerSharp/IResponse/) was served by a service worker; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L50)

<a id="PuppeteerSharp_Response_1_Headers_"></a>
<a id="PuppeteerSharp_Response_1_Headers"></a>

### Headers


```csharp
public Dictionary<string, string> Headers { get; protected init; }
```


An object with HTTP headers associated with the response. All header names are lower-case.
Duplicate header values are combined into a single comma-separated list except for
`Set-Cookie` that is separated by `\n`.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — The headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L38)

<a id="PuppeteerSharp_Response_1_Ok_"></a>
<a id="PuppeteerSharp_Response_1_Ok"></a>

### Ok


```csharp
public bool Ok { get; }
```


Contains a boolean stating whether the response was successful (status in the range 200-299) or not.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if ok; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L29)

<a id="PuppeteerSharp_Response_1_RemoteAddress_"></a>
<a id="PuppeteerSharp_Response_1_RemoteAddress"></a>

### RemoteAddress


```csharp
public RemoteAddress RemoteAddress { get; protected init; }
```


Remove server address.

**Value:** [RemoteAddress](/api/PuppeteerSharp/RemoteAddress/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L23)

<a id="PuppeteerSharp_Response_1_Request_"></a>
<a id="PuppeteerSharp_Response_1_Request"></a>

### Request


```csharp
protected TRequest Request { get; init; }
```


**Value:** `TRequest`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L59)

<a id="PuppeteerSharp_Response_1_SecurityDetails_"></a>
<a id="PuppeteerSharp_Response_1_SecurityDetails"></a>

### SecurityDetails


```csharp
public SecurityDetails SecurityDetails { get; protected init; }
```


Gets or sets the security details.

**Value:** [SecurityDetails](/api/PuppeteerSharp/SecurityDetails/) — The security details.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L47)

<a id="PuppeteerSharp_Response_1_Status_"></a>
<a id="PuppeteerSharp_Response_1_Status"></a>

### Status


```csharp
public HttpStatusCode Status { get; protected init; }
```


Contains the status code of the response.

**Value:** [HttpStatusCode](https://learn.microsoft.com/dotnet/api/system.net.httpstatuscode) — The status.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L32)

<a id="PuppeteerSharp_Response_1_StatusText_"></a>
<a id="PuppeteerSharp_Response_1_StatusText"></a>

### StatusText


```csharp
public string StatusText { get; protected init; }
```


Contains the status text of the response (e.g. usually an "OK" for a success).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The status text.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L35)

<a id="PuppeteerSharp_Response_1_Timing_"></a>
<a id="PuppeteerSharp_Response_1_Timing"></a>

### Timing


```csharp
public ResourceTiming Timing { get; protected init; }
```


Resource timing information for this response.

**Value:** [ResourceTiming](/api/PuppeteerSharp/ResourceTiming/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L56)

<a id="PuppeteerSharp_Response_1_Url_"></a>
<a id="PuppeteerSharp_Response_1_Url"></a>

### Url


```csharp
public string Url { get; protected init; }
```


Contains the URL of the response.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L26)

## Methods

<a id="PuppeteerSharp_Response_1_AsFetchResponseAsync_"></a>
<a id="PuppeteerSharp_Response_1_AsFetchResponseAsync"></a>

### AsFetchResponseAsync()


```csharp
public Task<HttpResponseMessage> AsFetchResponseAsync()
```


Converts the response to an [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage) instance.

**Returns:** [Task&lt;HttpResponseMessage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to an [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage) representation of the response.

Headers are copied to the new response, with multi-line `set-cookie` headers split into
individual header entries. For responses with null body statuses (101, 204, 205, 304), the body is omitted.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L85)

<a id="PuppeteerSharp_Response_1_BufferAsync_"></a>
<a id="PuppeteerSharp_Response_1_BufferAsync"></a>

### BufferAsync()


```csharp
public abstract ValueTask<byte[]> BufferAsync()
```


Returns a Task which resolves to a buffer with response body.

**Returns:** [ValueTask&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1) — A Task which resolves to a buffer with response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L65)

<a id="PuppeteerSharp_Response_1_JsonAsync_"></a>
<a id="PuppeteerSharp_Response_1_JsonAsync_System_Text_Json_JsonDocumentOptions_"></a>

### JsonAsync(JsonDocumentOptions)


```csharp
public Task<JsonDocument> JsonAsync(JsonDocumentOptions options = default)
```


Returns a Task which resolves to a [JsonDocument](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument) representation of response body.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [JsonDocumentOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocumentoptions) | Parsing options. |

**Returns:** [Task&lt;JsonDocument&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a [JsonDocument](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument) representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L71)

<a id="PuppeteerSharp_Response_1_JsonAsync__1_System_Text_Json_JsonSerializerOptions_"></a>

### `JsonAsync<T>(JsonSerializerOptions)`


```csharp
public Task<T> JsonAsync<T>(JsonSerializerOptions options = null)
```


Returns a Task which resolves to a &lt;code class="typeparamref"&gt;T&lt;/code&gt; representation of response body.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type of the response. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions) | Parsing options. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a &lt;code class="typeparamref"&gt;T&lt;/code&gt; representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L78)

<a id="PuppeteerSharp_Response_1_TextAsync_"></a>
<a id="PuppeteerSharp_Response_1_TextAsync"></a>

### TextAsync()


```csharp
public Task<string> TextAsync()
```


Returns a Task which resolves to a text representation of response body.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a text representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Response.cs#L68)
