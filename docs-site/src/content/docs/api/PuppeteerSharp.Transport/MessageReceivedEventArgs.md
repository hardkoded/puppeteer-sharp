---
title: "MessageReceivedEventArgs"
description: "Message received event arguments. MessageReceived."
editUrl: false
prev: false
next: false
sidebar:
  label: "MessageReceivedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_Transport_MessageReceivedEventArgs"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public class MessageReceivedEventArgs : EventArgs
```


Message received event arguments.
[MessageReceived](/api/PuppeteerSharp.Transport/IConnectionTransport/#PuppeteerSharp_Transport_IConnectionTransport_MessageReceived).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/MessageReceivedEventArgs.cs#L9)

## Constructors

<a id="PuppeteerSharp_Transport_MessageReceivedEventArgs__ctor_"></a>
<a id="PuppeteerSharp_Transport_MessageReceivedEventArgs__ctor_System_Byte___"></a>

### MessageReceivedEventArgs(byte[])


```csharp
public MessageReceivedEventArgs(byte[] message)
```


Initializes a new instance of the [MessageReceivedEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/MessageReceivedEventArgs.cs#L15)

## Properties

<a id="PuppeteerSharp_Transport_MessageReceivedEventArgs_Message_"></a>
<a id="PuppeteerSharp_Transport_MessageReceivedEventArgs_Message"></a>

### Message


```csharp
public byte[] Message { get; }
```


Transport message.

**Value:** [byte\[\]](https://learn.microsoft.com/dotnet/api/system.byte)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/MessageReceivedEventArgs.cs#L20)
