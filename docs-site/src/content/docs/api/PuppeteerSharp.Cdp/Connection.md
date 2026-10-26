---
title: "Connection"
description: "A connection handles the communication with a Chromium browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "Connection"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_Connection"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public sealed class Connection : IDisposable, ICDPConnection
```


A connection handles the communication with a Chromium browser.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IDisposable](https://learn.microsoft.com/dotnet/api/system.idisposable), [ICDPConnection](/api/PuppeteerSharp/ICDPConnection/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L21)

## Properties

<a id="PuppeteerSharp_Cdp_Connection_CloseReason_"></a>
<a id="PuppeteerSharp_Cdp_Connection_CloseReason"></a>

### CloseReason


```csharp
public string CloseReason { get; }
```


Connection close reason.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L88)

<a id="PuppeteerSharp_Cdp_Connection_Delay_"></a>
<a id="PuppeteerSharp_Cdp_Connection_Delay"></a>

### Delay


```csharp
public int Delay { get; }
```


Gets the sleep time when a message is received.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — The delay.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L71)

<a id="PuppeteerSharp_Cdp_Connection_IsClosed_"></a>
<a id="PuppeteerSharp_Cdp_Connection_IsClosed"></a>

### IsClosed


```csharp
public bool IsClosed { get; }
```


Gets a value indicating whether this [Connection](./) is closed.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if is closed; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L83)

<a id="PuppeteerSharp_Cdp_Connection_LoggerFactory_"></a>
<a id="PuppeteerSharp_Cdp_Connection_LoggerFactory"></a>

### LoggerFactory


```csharp
public ILoggerFactory LoggerFactory { get; }
```


Gets the logger factory.

**Value:** [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) — The logger factory.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L94)

<a id="PuppeteerSharp_Cdp_Connection_Transport_"></a>
<a id="PuppeteerSharp_Cdp_Connection_Transport"></a>

### Transport


```csharp
public IConnectionTransport Transport { get; }
```


Gets the Connection transport.

**Value:** [IConnectionTransport](/api/PuppeteerSharp.Transport/IConnectionTransport/) — Connection transport.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L77)

<a id="PuppeteerSharp_Cdp_Connection_Url_"></a>
<a id="PuppeteerSharp_Cdp_Connection_Url"></a>

### Url


```csharp
public string Url { get; }
```


Gets the WebSocket URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L65)

## Events

<a id="PuppeteerSharp_Cdp_Connection_Disconnected"></a>

### Disconnected


```csharp
public event EventHandler Disconnected
```


Occurs when the connection is closed.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L50)

<a id="PuppeteerSharp_Cdp_Connection_MessageReceived"></a>

### MessageReceived


```csharp
public event EventHandler<MessageEventArgs> MessageReceived
```


Occurs when a message from chromium is received.

**Value:** [EventHandler&lt;MessageEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L55)

## Methods

<a id="PuppeteerSharp_Cdp_Connection_Dispose_"></a>
<a id="PuppeteerSharp_Cdp_Connection_Dispose"></a>

### Dispose()


```csharp
public void Dispose()
```


Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L105)

<a id="PuppeteerSharp_Cdp_Connection_SendAsync_"></a>
<a id="PuppeteerSharp_Cdp_Connection_SendAsync_System_String_System_Object_System_Boolean_PuppeteerSharp_CommandOptions_"></a>

### SendAsync(string, object, bool, CommandOptions)


```csharp
public Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
```


Protocol methods can be called with this method.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `method` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The method name. |
| `args` | [object](https://learn.microsoft.com/dotnet/api/system.object) | The method args. |
| `waitForCallback` | [bool](https://learn.microsoft.com/dotnet/api/system.boolean) | If `true` the method will return a task to be completed when the message is confirmed by Chromium. If `false` the task will be considered complete after sending the message to Chromium. |
| `options` | [CommandOptions](/api/PuppeteerSharp/CommandOptions/) | The options. |

**Returns:** [Task&lt;JsonElement?&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | If the [Connection](./) is closed. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L112)

<a id="PuppeteerSharp_Cdp_Connection_SendAsync__1_System_String_System_Object_PuppeteerSharp_CommandOptions_"></a>

### `SendAsync<T>(string, object, CommandOptions)`


```csharp
public Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null)
```


Protocol methods can be called with this method.

**Type parameters**

| Name | Description |
| --- | --- |
| `T` | Return type. |

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `method` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The method name. |
| `args` | [object](https://learn.microsoft.com/dotnet/api/system.object) | The method args. |
| `options` | [CommandOptions](/api/PuppeteerSharp/CommandOptions/) | The options. |

**Returns:** [Task&lt;T&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — The task.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/Connection.cs#L155)
