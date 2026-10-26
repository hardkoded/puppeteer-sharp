---
title: "ResponseCreatedEventArgs"
description: "Response arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "ResponseCreatedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_ResponseCreatedEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ResponseCreatedEventArgs : EventArgs
```


[Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseCreatedEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_ResponseCreatedEventArgs__ctor_"></a>
<a id="PuppeteerSharp_ResponseCreatedEventArgs__ctor_PuppeteerSharp_IResponse_"></a>

### ResponseCreatedEventArgs(IResponse)


```csharp
public ResponseCreatedEventArgs(IResponse response)
```


[Response](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Response) arguments.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `response` | [IResponse](/api/PuppeteerSharp/IResponse/) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseCreatedEventArgs.cs#L8)

## Properties

<a id="PuppeteerSharp_ResponseCreatedEventArgs_Response_"></a>
<a id="PuppeteerSharp_ResponseCreatedEventArgs_Response"></a>

### Response


```csharp
public IResponse Response { get; }
```


Gets the response.

**Value:** [IResponse](/api/PuppeteerSharp/IResponse/) — The response.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ResponseCreatedEventArgs.cs#L14)
