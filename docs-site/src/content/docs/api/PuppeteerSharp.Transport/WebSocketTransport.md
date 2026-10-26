---
title: "WebSocketTransport"
description: "Default web socket transport."
editUrl: false
prev: false
next: false
sidebar:
  label: "WebSocketTransport"
  hidden: true
---

<a id="PuppeteerSharp_Transport_WebSocketTransport"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public class WebSocketTransport : IConnectionTransport, IDisposable
```


Default web socket transport.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L16)

## Constructors

<a id="PuppeteerSharp_Transport_WebSocketTransport__ctor_"></a>
<a id="PuppeteerSharp_Transport_WebSocketTransport__ctor_System_Net_WebSockets_WebSocket_PuppeteerSharp_Transport_TransportTaskScheduler_System_Boolean_"></a>

### WebSocketTransport(WebSocket, TransportTaskScheduler, bool)


```csharp
protected WebSocketTransport(WebSocket client, TransportTaskScheduler scheduler, bool queueRequests)
```


Initializes a new instance of the [WebSocketTransport](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `client` | [WebSocket](https://learn.microsoft.com/dotnet/api/system.net.websockets.websocket) | The web socket. |
| `scheduler` | [TransportTaskScheduler](/api/PuppeteerSharp.Transport/TransportTaskScheduler/) | The scheduler to use for long-running tasks. |
| `queueRequests` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether requests should be queued. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L48)

## Fields

<a id="PuppeteerSharp_Transport_WebSocketTransport_DefaultTransportFactory"></a>

### DefaultTransportFactory


```csharp
public static readonly TransportFactory DefaultTransportFactory
```


Gets the default [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/). This factory does not support Windows 7.

**Value:** [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L21)

<a id="PuppeteerSharp_Transport_WebSocketTransport_DefaultTransportScheduler"></a>

### DefaultTransportScheduler


```csharp
public static readonly TransportTaskScheduler DefaultTransportScheduler
```


Gets the default [TransportTaskScheduler](/api/PuppeteerSharp.Transport/TransportTaskScheduler/).

**Value:** [TransportTaskScheduler](/api/PuppeteerSharp.Transport/TransportTaskScheduler/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L31)

<a id="PuppeteerSharp_Transport_WebSocketTransport_DefaultWebSocketFactory"></a>

### DefaultWebSocketFactory


```csharp
public static readonly WebSocketFactory DefaultWebSocketFactory
```


Gets the default [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/).

**Value:** [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L26)

## Properties

<a id="PuppeteerSharp_Transport_WebSocketTransport_IsClosed_"></a>
<a id="PuppeteerSharp_Transport_WebSocketTransport_IsClosed"></a>

### IsClosed


```csharp
public bool IsClosed { get; }
```


Gets a value indicating whether this [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L73)

## Events

<a id="PuppeteerSharp_Transport_WebSocketTransport_Closed"></a>

### Closed


```csharp
public event EventHandler<TransportClosedEventArgs> Closed
```


Occurs when the transport is closed.

**Value:** [EventHandler&lt;TransportClosedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L63)

<a id="PuppeteerSharp_Transport_WebSocketTransport_MessageReceived"></a>

### MessageReceived


```csharp
public event EventHandler<MessageReceivedEventArgs> MessageReceived
```


Occurs when a message is received.

**Value:** [EventHandler&lt;MessageReceivedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L68)

## Methods

<a id="PuppeteerSharp_Transport_WebSocketTransport_Dispose_"></a>
<a id="PuppeteerSharp_Transport_WebSocketTransport_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L99)

<a id="PuppeteerSharp_Transport_WebSocketTransport_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Close the WebSocketTransport.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Indicates whether disposal was initiated by [Dispose()](#PuppeteerSharp_Transport_WebSocketTransport_Dispose) operation. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L109)

<a id="PuppeteerSharp_Transport_WebSocketTransport_SendAsync_"></a>
<a id="PuppeteerSharp_Transport_WebSocketTransport_SendAsync_System_Byte___"></a>

### SendAsync(byte[])


```csharp
public Task SendAsync(byte[] message)
```


Sends a message using the transport.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte) | Message to send. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L76)

<a id="PuppeteerSharp_Transport_WebSocketTransport_StopReading_"></a>
<a id="PuppeteerSharp_Transport_WebSocketTransport_StopReading"></a>

### StopReading()


```csharp
public void StopReading()
```


Stops reading incoming data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/WebSocketTransport.cs#L86)
