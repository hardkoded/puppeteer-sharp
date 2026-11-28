---
title: "IConnectionTransport"
description: "Connection transport abstraction."
editUrl: false
prev: false
next: false
sidebar:
  label: "IConnectionTransport"
  hidden: true
---

<a id="PuppeteerSharp_Transport_IConnectionTransport"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public interface IConnectionTransport : IDisposable
```


Connection transport abstraction.

**Derived:** [PipeTransport](/api/PuppeteerSharp.Transport/PipeTransport/), [WebSocketTransport](/api/PuppeteerSharp.Transport/WebSocketTransport/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L9)

## Properties

<a id="PuppeteerSharp_Transport_IConnectionTransport_IsClosed_"></a>
<a id="PuppeteerSharp_Transport_IConnectionTransport_IsClosed"></a>

### IsClosed


```csharp
bool IsClosed { get; }
```


Gets a value indicating whether this [IConnectionTransport](./) is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L24)

## Events

<a id="PuppeteerSharp_Transport_IConnectionTransport_Closed"></a>

### Closed


```csharp
event EventHandler<TransportClosedEventArgs> Closed
```


Occurs when the transport is closed.

**Value:** [EventHandler&lt;TransportClosedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L14)

<a id="PuppeteerSharp_Transport_IConnectionTransport_MessageReceived"></a>

### MessageReceived


```csharp
event EventHandler<MessageReceivedEventArgs> MessageReceived
```


Occurs when a message is received.

**Value:** [EventHandler&lt;MessageReceivedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L19)

## Methods

<a id="PuppeteerSharp_Transport_IConnectionTransport_SendAsync_"></a>
<a id="PuppeteerSharp_Transport_IConnectionTransport_SendAsync_System_Byte___"></a>

### SendAsync(byte[])


```csharp
Task SendAsync(byte[] message)
```


Sends a message using the transport.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte) | Message to send. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L36)

<a id="PuppeteerSharp_Transport_IConnectionTransport_StopReading_"></a>
<a id="PuppeteerSharp_Transport_IConnectionTransport_StopReading"></a>

### StopReading()


```csharp
void StopReading()
```


Stops reading incoming data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/IConnectionTransport.cs#L29)
