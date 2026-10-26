---
title: "PuppeteerSharp.Transport"
description: "WebSocket and pipe transports."
editUrl: false
sidebar:
  label: "PuppeteerSharp.Transport"
---

[API reference](/api/)

WebSocket and pipe transports.

## Classes

| Type | Summary |
| --- | --- |
| [MessageReceivedEventArgs](/api/PuppeteerSharp.Transport/MessageReceivedEventArgs/) | Message received event arguments. MessageReceived. |
| [PipeTransport](/api/PuppeteerSharp.Transport/PipeTransport/) | Transport implementation using pipes for browser communication. Used with Chrome's --remote-debugging-pipe flag, which communicates via file descriptors 3 (browser reads) and 4 (browser writes). Messages are null-terminated (\0) JSON strings. |
| [TransportClosedEventArgs](/api/PuppeteerSharp.Transport/TransportClosedEventArgs/) | Closed. |
| [WebSocketTransport](/api/PuppeteerSharp.Transport/WebSocketTransport/) | Default web socket transport. |

## Interfaces

| Type | Summary |
| --- | --- |
| [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) | Connection transport abstraction. |

## Delegates

| Type | Summary |
| --- | --- |
| [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/) | Delegate for creation of IConnectionTransport instances. |
| [TransportTaskScheduler](/api/PuppeteerSharp.Transport/TransportTaskScheduler/) | Delegate for scheduling of long-running transport tasks. |
| [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/) | Delegate for creation of WebSocket instances. |
