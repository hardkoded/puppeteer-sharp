---
title: "WebSocketFactory"
description: "Delegate for creation of WebSocket instances."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebSocketFactory"
  hidden: true
---

<a id="PuppeteerSharp_Transport_WebSocketFactory"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public delegate Task<WebSocket> WebSocketFactory(Uri url, IConnectionOptions options, CancellationToken cancellationToken)
```


Delegate for creation of [WebSocket](https://learn.microsoft.com/dotnet/api/system.net.websockets.websocket) instances.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [Uri](https://learn.microsoft.com/dotnet/api/system.uri) | Chromium URL. |
| `options` | [IConnectionOptions](/api/PuppeteerSharp/IConnectionOptions/) | Connection options. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | A cancellation token. |

**Returns:** [Task&lt;WebSocket&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` instance for the asynchronous socket create and connect operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketFactory.cs#L15)
