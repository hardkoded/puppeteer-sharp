---
title: "IConnectionOptions"
description: "Options for Connection creation."
editUrl: false
prev: false
next: false
sidebar:
  label: "IConnectionOptions"
  hidden: true
---

<a id="PuppeteerSharp_IConnectionOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface IConnectionOptions
```


Options for [Connection](/api/PuppeteerSharp.Cdp/Connection/) creation.

**Derived:** [ConnectOptions](/api/PuppeteerSharp/ConnectOptions/), [LaunchOptions](/api/PuppeteerSharp/LaunchOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L12)

## Properties

<a id="PuppeteerSharp_IConnectionOptions_EnqueueAsyncMessages_"></a>
<a id="PuppeteerSharp_IConnectionOptions_EnqueueAsyncMessages"></a>

### EnqueueAsyncMessages


```csharp
bool EnqueueAsyncMessages { get; set; }
```


Affects how responses to `CommandOptions)` are returned to the caller.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L38)

<a id="PuppeteerSharp_IConnectionOptions_EnqueueTransportMessages_"></a>
<a id="PuppeteerSharp_IConnectionOptions_EnqueueTransportMessages"></a>

### EnqueueTransportMessages


```csharp
bool EnqueueTransportMessages { get; set; }
```


If not [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this will be use to determine is the default [WebSocketTransport](/api/PuppeteerSharp.Transport/WebSocketTransport/) will enqueue messages.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L33)

<a id="PuppeteerSharp_IConnectionOptions_Headers_"></a>
<a id="PuppeteerSharp_IConnectionOptions_Headers"></a>

### Headers


```csharp
[Obsolete("Use WsOptions.Headers via IConnectionOptions.WsOptions instead. When both are set, WsOptions.Headers wins.")]
Dictionary<string, string> Headers { get; set; }
```


Headers that should be sent with connection-related HTTP and WebSocket requests.

**Value:** [Dictionary&lt;string, string&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.dictionary-2)

:::caution[Obsolete]
This API is obsolete.
:::

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L60)

<a id="PuppeteerSharp_IConnectionOptions_ProtocolTimeout_"></a>
<a id="PuppeteerSharp_IConnectionOptions_ProtocolTimeout"></a>

### ProtocolTimeout


```csharp
int ProtocolTimeout { get; set; }
```


Timeout setting for individual protocol (CDP) calls.
Defaults to 180_000.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L50)

<a id="PuppeteerSharp_IConnectionOptions_SlowMo_"></a>
<a id="PuppeteerSharp_IConnectionOptions_SlowMo"></a>

### SlowMo


```csharp
int SlowMo { get; set; }
```


Slows down Puppeteer operations by the specified amount of milliseconds. Useful so that you can see what is going on.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L17)

<a id="PuppeteerSharp_IConnectionOptions_TargetFilter_"></a>
<a id="PuppeteerSharp_IConnectionOptions_TargetFilter"></a>

### TargetFilter


```csharp
Func<Target, bool> TargetFilter { get; set; }
```


Callback to decide if Puppeteer should connect to a given target or not.
Supported only with CDP.

**Value:** [Func&lt;Target, bool&gt;](https://learn.microsoft.com/dotnet/api/system.func-2)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L44)

<a id="PuppeteerSharp_IConnectionOptions_TransportFactory_"></a>
<a id="PuppeteerSharp_IConnectionOptions_TransportFactory"></a>

### TransportFactory


```csharp
TransportFactory TransportFactory { get; set; }
```


Optional factory for [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) implementations.

**Value:** [TransportFactory](/api/PuppeteerSharp.Transport/TransportFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L28)

<a id="PuppeteerSharp_IConnectionOptions_WebSocketFactory_"></a>
<a id="PuppeteerSharp_IConnectionOptions_WebSocketFactory"></a>

### WebSocketFactory


```csharp
WebSocketFactory WebSocketFactory { get; set; }
```


Optional factory for [WebSocket](https://learn.microsoft.com/dotnet/api/system.net.websockets.websocket) implementations.
If [PuppeteerSharp.Transport](/api/PuppeteerSharp.Transport/) is set this property will be ignored.

**Value:** [WebSocketFactory](/api/PuppeteerSharp.Transport/WebSocketFactory/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L23)

<a id="PuppeteerSharp_IConnectionOptions_WsOptions_"></a>
<a id="PuppeteerSharp_IConnectionOptions_WsOptions"></a>

### WsOptions


```csharp
WsOptions WsOptions { get; set; }
```


Options for the WebSocket connection to the browser.

**Value:** [WsOptions](/api/PuppeteerSharp/WsOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/IConnectionOptions.cs#L55)
