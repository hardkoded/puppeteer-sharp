---
title: "TransportFactory"
description: "Delegate for creation of IConnectionTransport instances."
editUrl: false
prev: false
next: false
sidebar:
  label: "TransportFactory"
  hidden: true
---

<a id="PuppeteerSharp_Transport_TransportFactory"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public delegate Task<IConnectionTransport> TransportFactory(Uri url, IConnectionOptions options, CancellationToken cancellationToken)
```


Delegate for creation of [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) instances.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `url` | [Uri](https://learn.microsoft.com/dotnet/api/system.uri) | Chromium URL. |
| `options` | [IConnectionOptions](/api/PuppeteerSharp/IConnectionOptions/) | Connection options. |
| `cancellationToken` | [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken) | A cancellation token. |

**Returns:** [Task&lt;IConnectionTransport&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — A `Task%601` instance for the asynchronous socket create and connect operation.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/TransportFactory.cs#L14)
