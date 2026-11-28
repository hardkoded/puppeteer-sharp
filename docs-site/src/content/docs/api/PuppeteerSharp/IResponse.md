---
title: "IResponse"
description: "IResponse class represents responses which are received by page."
editUrl: false
prev: false
next: false
sidebar:
  label: "IResponse"
  hidden: true
---

<a id="PuppeteerSharp_IResponse"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IResponse
```


[IResponse](./) class represents responses which are received by page.

**Derived:** [BidiHttpResponse](/api/PuppeteerSharp.Bidi/BidiHttpResponse/), [CdpHttpResponse](/api/PuppeteerSharp.Cdp/CdpHttpResponse/), [Response&lt;TRequest&gt;](/api/PuppeteerSharp/Response-1/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L18)

## Properties

<a id="PuppeteerSharp_IResponse_EncodedDataLength_"></a>
<a id="PuppeteerSharp_IResponse_EncodedDataLength"></a>

### EncodedDataLength


```csharp
long EncodedDataLength { get; }
```


Total number of bytes received for this request over the network.

**Value:** [long](https://learn.microsoft.com/dotnet/api/system.int64)

The value is updated when the response finishes loading. During the `Response` event, it only covers
the bytes received up to the response headers. To get the total encoded transfer size (headers + compressed
body), read it in a `RequestFinished` handler. Awaiting [BufferAsync()](#PuppeteerSharp_IResponse_BufferAsync) does not guarantee the final size is available.
If the request fails after the response is received, the value is not updated. If the browser does not
report a size, e.g. for `data:` or `file:` URLs, the value is `0`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L99)

<a id="PuppeteerSharp_IResponse_Frame_"></a>
<a id="PuppeteerSharp_IResponse_Frame"></a>

### Frame


```csharp
IFrame Frame { get; }
```


A [Frame](#PuppeteerSharp_IResponse_Frame) that initiated this request. Or null if navigating to error pages.

**Value:** [IFrame](/api/PuppeteerSharp/IFrame/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L82)

<a id="PuppeteerSharp_IResponse_FromCache_"></a>
<a id="PuppeteerSharp_IResponse_FromCache"></a>

### FromCache


```csharp
bool FromCache { get; }
```


True if the response was served from either the browser's disk cache or memory cache.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L54)

<a id="PuppeteerSharp_IResponse_FromServiceWorker_"></a>
<a id="PuppeteerSharp_IResponse_FromServiceWorker"></a>

### FromServiceWorker


```csharp
bool FromServiceWorker { get; }
```


Gets a value indicating whether the [IResponse](./) was served by a service worker.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if the [IResponse](./) was served by a service worker; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L66)

<a id="PuppeteerSharp_IResponse_Headers_"></a>
<a id="PuppeteerSharp_IResponse_Headers"></a>

### Headers


```csharp
Dictionary<string, string> Headers { get; }
```


An object with HTTP headers associated with the response. All header names are lower-case.
Duplicate header values are combined into a single comma-separated list except for
`Set-Cookie` that is separated by `\n`.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2) — The headers.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L31)

<a id="PuppeteerSharp_IResponse_Ok_"></a>
<a id="PuppeteerSharp_IResponse_Ok"></a>

### Ok


```csharp
bool Ok { get; }
```


Contains a boolean stating whether the response was successful (status in the range 200-299) or not.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if ok; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L43)

<a id="PuppeteerSharp_IResponse_RemoteAddress_"></a>
<a id="PuppeteerSharp_IResponse_RemoteAddress"></a>

### RemoteAddress


```csharp
RemoteAddress RemoteAddress { get; }
```


Remove server address.

**Value:** [RemoteAddress](/api/PuppeteerSharp/RemoteAddress/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L77)

<a id="PuppeteerSharp_IResponse_Request_"></a>
<a id="PuppeteerSharp_IResponse_Request"></a>

### Request


```csharp
IRequest Request { get; }
```


A matching [Request](#PuppeteerSharp_IResponse_Request) object.

**Value:** [IRequest](/api/PuppeteerSharp/IRequest/) — The request.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L49)

<a id="PuppeteerSharp_IResponse_SecurityDetails_"></a>
<a id="PuppeteerSharp_IResponse_SecurityDetails"></a>

### SecurityDetails


```csharp
SecurityDetails SecurityDetails { get; }
```


Gets or sets the security details.

**Value:** [SecurityDetails](/api/PuppeteerSharp/SecurityDetails/) — The security details.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L60)

<a id="PuppeteerSharp_IResponse_Status_"></a>
<a id="PuppeteerSharp_IResponse_Status"></a>

### Status


```csharp
HttpStatusCode Status { get; }
```


Contains the status code of the response.

**Value:** [HttpStatusCode](https://learn.microsoft.com/dotnet/api/system.net.httpstatuscode) — The status.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L37)

<a id="PuppeteerSharp_IResponse_StatusText_"></a>
<a id="PuppeteerSharp_IResponse_StatusText"></a>

### StatusText


```csharp
string StatusText { get; }
```


Contains the status text of the response (e.g. usually an "OK" for a success).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The status text.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L72)

<a id="PuppeteerSharp_IResponse_Timing_"></a>
<a id="PuppeteerSharp_IResponse_Timing"></a>

### Timing


```csharp
ResourceTiming Timing { get; }
```


Resource timing information for this response.

**Value:** [ResourceTiming](/api/PuppeteerSharp/ResourceTiming/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L87)

<a id="PuppeteerSharp_IResponse_Url_"></a>
<a id="PuppeteerSharp_IResponse_Url"></a>

### Url


```csharp
string Url { get; }
```


Contains the URL of the response.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L23)

## Methods

<a id="PuppeteerSharp_IResponse_AsFetchResponseAsync_"></a>
<a id="PuppeteerSharp_IResponse_AsFetchResponseAsync"></a>

### AsFetchResponseAsync()


```csharp
Task<HttpResponseMessage> AsFetchResponseAsync()
```


Converts the response to an [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage) instance.

**Returns:** [Task&lt;HttpResponseMessage&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to an [HttpResponseMessage](https://learn.microsoft.com/dotnet/api/system.net.http.httpresponsemessage) representation of the response.

Headers are copied to the new response, with multi-line `set-cookie` headers split into
individual header entries. For responses with null body statuses (101, 204, 205, 304), the body is omitted.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L148)

<a id="PuppeteerSharp_IResponse_BufferAsync_"></a>
<a id="PuppeteerSharp_IResponse_BufferAsync"></a>

### BufferAsync()


```csharp
ValueTask<byte[]> BufferAsync()
```


Returns a Task which resolves to a buffer with response body.

**Returns:** [ValueTask&lt;byte\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask-1) — A Task which resolves to a buffer with response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L105)

<a id="PuppeteerSharp_IResponse_JsonAsync_"></a>
<a id="PuppeteerSharp_IResponse_JsonAsync_System_Text_Json_JsonDocumentOptions_"></a>

### JsonAsync(JsonDocumentOptions)


```csharp
Task<JsonDocument> JsonAsync(JsonDocumentOptions options = default)
```


Returns a Task which resolves to a [JsonDocument](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument) representation of response body.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [JsonDocumentOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocumentoptions) | Parsing options. |

**Returns:** [Task&lt;JsonDocument&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a [JsonDocument](https://learn.microsoft.com/dotnet/api/system.text.json.jsondocument) representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L129)

<a id="PuppeteerSharp_IResponse_JsonAsync__1_System_Text_Json_JsonSerializerOptions_"></a>

### `JsonAsync<T>(JsonSerializerOptions)`


```csharp
Task<T> JsonAsync<T>(JsonSerializerOptions options = null)
```


Returns a Task which resolves to a `T` representation of response body.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | The type of the response. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [JsonSerializerOptions](https://learn.microsoft.com/dotnet/api/system.text.json.jsonserializeroptions) | Parsing options. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a `T` representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L138)

<a id="PuppeteerSharp_IResponse_StreamAsync_"></a>
<a id="PuppeteerSharp_IResponse_StreamAsync"></a>

### StreamAsync()


```csharp
Task<Stream> StreamAsync()
```


Returns a Task which resolves to a stream of the response body, with chunks delivered as they arrive from the network.

**Returns:** [Task&lt;Stream&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a stream of the response body.

Call this from the [Response](/api/PuppeteerSharp/Page/#PuppeteerSharp_Page_Response) event to receive the body while it is still loading.
If the body has already finished loading, the stream contains the whole body as a single chunk.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L115)

<a id="PuppeteerSharp_IResponse_TextAsync_"></a>
<a id="PuppeteerSharp_IResponse_TextAsync"></a>

### TextAsync()


```csharp
Task<string> TextAsync()
```


Returns a Task which resolves to a text representation of response body.

**Returns:** [Task&lt;string&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A Task which resolves to a text representation of response body.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IResponse.cs#L121)
