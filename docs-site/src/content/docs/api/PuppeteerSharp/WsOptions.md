---
title: "WsOptions"
description: "Options for the WebSocket connection to the browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "WsOptions"
  hidden: true
---

<a id="PuppeteerSharp_WsOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WsOptions
```


Options for the WebSocket connection to the browser.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L8)

## Fields

<a id="PuppeteerSharp_WsOptions_DefaultKeepAliveIntervalMs"></a>

### DefaultKeepAliveIntervalMs


```csharp
public const int DefaultKeepAliveIntervalMs = 30000
```


Default ping period in milliseconds when [KeepAlive](#PuppeteerSharp_WsOptions_KeepAlive) is enabled.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L13)

## Properties

<a id="PuppeteerSharp_WsOptions_HandshakeTimeout_"></a>
<a id="PuppeteerSharp_WsOptions_HandshakeTimeout"></a>

### HandshakeTimeout


```csharp
public int? HandshakeTimeout { get; set; }
```


Time in milliseconds to wait for the WebSocket opening handshake before failing.
Without it, connecting to a host that accepts the TCP connection but never answers
the upgrade request waits until the operating system gives up.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L43)

<a id="PuppeteerSharp_WsOptions_Headers_"></a>
<a id="PuppeteerSharp_WsOptions_Headers"></a>

### Headers


```csharp
public Dictionary<string, string> Headers { get; set; }
```


Headers to use for the web socket connection.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L18)

<a id="PuppeteerSharp_WsOptions_KeepAlive_"></a>
<a id="PuppeteerSharp_WsOptions_KeepAlive"></a>

### KeepAlive


```csharp
public bool KeepAlive { get; set; }
```


Whether to send WebSocket pings and drop the connection when a pong does
not come back within the same interval. Detects a connection that died
without a close frame, which otherwise leaves calls hanging until
[ProtocolTimeout](/api/PuppeteerSharp/IConnectionOptions/#PuppeteerSharp_IConnectionOptions_ProtocolTimeout).

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Dead-connection detection requires .NET 9 or later. On earlier runtimes,
enabling keep-alive only configures the WebSocket keep-alive interval.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L30)

<a id="PuppeteerSharp_WsOptions_KeepAliveIntervalMs_"></a>
<a id="PuppeteerSharp_WsOptions_KeepAliveIntervalMs"></a>

### KeepAliveIntervalMs


```csharp
public int? KeepAliveIntervalMs { get; set; }
```


Ping period in milliseconds. Only used when [KeepAlive](#PuppeteerSharp_WsOptions_KeepAlive) is set.
Defaults to [DefaultKeepAliveIntervalMs](#PuppeteerSharp_WsOptions_DefaultKeepAliveIntervalMs).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WsOptions.cs#L36)
