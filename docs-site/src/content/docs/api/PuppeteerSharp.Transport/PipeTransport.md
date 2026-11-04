---
title: "PipeTransport"
description: "Transport implementation using pipes for browser communication. Used with Chrome's --remote-debugging-pipe flag, which communicates via file descriptors 3 (b..."
editUrl: false
prev: false
next: false
sidebar:
  label: "PipeTransport"
  hidden: true
---

<a id="PuppeteerSharp_Transport_PipeTransport"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public class PipeTransport : IConnectionTransport, IDisposable
```


Transport implementation using pipes for browser communication.
Used with Chrome's `--remote-debugging-pipe` flag, which communicates
via file descriptors 3 (browser reads) and 4 (browser writes).
Messages are null-terminated (`\0`) JSON strings.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/), [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L16)

## Constructors

<a id="PuppeteerSharp_Transport_PipeTransport__ctor_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport__ctor_System_IO_Stream_System_IO_Stream_"></a>

### PipeTransport(Stream, Stream)


```csharp
public PipeTransport(Stream pipeWrite, Stream pipeRead)
```


Initializes a new instance of the [PipeTransport](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `pipeWrite` | [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) | The stream to write messages to (browser reads from this). |
| `pipeRead` | [Stream](https://learn.microsoft.com/dotnet/api/system.io.stream) | The stream to read messages from (browser writes to this). |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L31)

## Properties

<a id="PuppeteerSharp_Transport_PipeTransport_IsClosed_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport_IsClosed"></a>

### IsClosed


```csharp
public bool IsClosed { get; }
```


Gets a value indicating whether this [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L44)

## Events

<a id="PuppeteerSharp_Transport_PipeTransport_Closed"></a>

### Closed


```csharp
public event EventHandler<TransportClosedEventArgs> Closed
```


Occurs when the transport is closed.

**Value:** [EventHandler&lt;TransportClosedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L38)

<a id="PuppeteerSharp_Transport_PipeTransport_MessageReceived"></a>

### MessageReceived


```csharp
public event EventHandler<MessageReceivedEventArgs> MessageReceived
```


Occurs when a message is received.

**Value:** [EventHandler&lt;MessageReceivedEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L41)

## Methods

<a id="PuppeteerSharp_Transport_PipeTransport_Dispose_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L109)

<a id="PuppeteerSharp_Transport_PipeTransport_Dispose_System_Boolean_"></a>

### Dispose(bool)


```csharp
protected virtual void Dispose(bool disposing)
```


Releases resources used by the [PipeTransport](./).

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `disposing` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | Whether disposal was initiated by [Dispose()](#PuppeteerSharp_Transport_PipeTransport_Dispose). |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L119)

<a id="PuppeteerSharp_Transport_PipeTransport_SendAsync_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport_SendAsync_System_Byte___"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L59)

<a id="PuppeteerSharp_Transport_PipeTransport_Start_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport_Start"></a>

### Start()


```csharp
public void Start()
```


Starts the background receive loop.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L49)

<a id="PuppeteerSharp_Transport_PipeTransport_StopReading_"></a>
<a id="PuppeteerSharp_Transport_PipeTransport_StopReading"></a>

### StopReading()


```csharp
public void StopReading()
```


Stops reading incoming data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/PipeTransport.cs#L98)
