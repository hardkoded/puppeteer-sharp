---
title: "TransportClosedEventArgs"
description: "Closed."
editUrl: false
prev: false
next: false
sidebar:
  label: "TransportClosedEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_Transport_TransportClosedEventArgs"></a>

Namespace: [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/)


```csharp
public class TransportClosedEventArgs : EventArgs
```


[Closed](/api/PuppeteerSharp.Transport/IConnectionTransport/#PuppeteerSharp_Transport_IConnectionTransport_Closed).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/TransportClosedEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_Transport_TransportClosedEventArgs__ctor_"></a>
<a id="PuppeteerSharp_Transport_TransportClosedEventArgs__ctor_System_String_"></a>

### TransportClosedEventArgs(string)


```csharp
public TransportClosedEventArgs(string closeReason)
```


Initializes a new instance of the [TransportClosedEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `closeReason` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Close reason. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/TransportClosedEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_Transport_TransportClosedEventArgs_CloseReason_"></a>
<a id="PuppeteerSharp_Transport_TransportClosedEventArgs_CloseReason"></a>

### CloseReason


```csharp
public string CloseReason { get; set; }
```


Gets or sets the close reason.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Transport/TransportClosedEventArgs.cs#L19)
