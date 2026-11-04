---
title: "RequestEventArgs"
description: "Arguments used by IPage events."
editUrl: false
prev: false
next: false
sidebar:
  label: "RequestEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_RequestEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class RequestEventArgs : EventArgs
```


Arguments used by [IPage](/api/PuppeteerSharp/IPage/) events.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestEventArgs.cs#L11)

## Constructors

<a id="PuppeteerSharp_RequestEventArgs__ctor_"></a>
<a id="PuppeteerSharp_RequestEventArgs__ctor_PuppeteerSharp_IRequest_"></a>

### RequestEventArgs(IRequest)


```csharp
public RequestEventArgs(IRequest request)
```


Arguments used by [IPage](/api/PuppeteerSharp/IPage/) events.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `request` | [IRequest](/api/PuppeteerSharp/IRequest/) |  |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestEventArgs.cs#L11)

## Properties

<a id="PuppeteerSharp_RequestEventArgs_Request_"></a>
<a id="PuppeteerSharp_RequestEventArgs_Request"></a>

### Request


```csharp
public IRequest Request { get; }
```


Gets the request.

**Value:** [IRequest](/api/PuppeteerSharp/IRequest/) — The request.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RequestEventArgs.cs#L17)
