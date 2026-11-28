---
title: "ICDPConnection"
description: "An ICDPConnection is an object able to send and receive messages from the browser."
editUrl: false
prev: false
next: false
sidebar:
  label: "ICDPConnection"
  hidden: true
---

<a id="PuppeteerSharp_ICDPConnection"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface ICDPConnection
```


An ICDPConnection is an object able to send and receive messages from the browser.

**Derived:** [BidiCdpSession](/api/PuppeteerSharp.Bidi/BidiCdpSession/), [CDPSession](/api/PuppeteerSharp/CDPSession/), [CdpCDPSession](/api/PuppeteerSharp.Cdp/CdpCDPSession/), [Connection](/api/PuppeteerSharp.Cdp/Connection/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPConnection.cs#L11)

## Events

<a id="PuppeteerSharp_ICDPConnection_MessageReceived"></a>

### MessageReceived


```csharp
event EventHandler<MessageEventArgs> MessageReceived
```


Occurs when message received from Chromium.

**Value:** [EventHandler&lt;MessageEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPConnection.cs#L16)

## Methods

<a id="PuppeteerSharp_ICDPConnection_SendAsync_"></a>
<a id="PuppeteerSharp_ICDPConnection_SendAsync_System_String_System_Object_System_Boolean_PuppeteerSharp_CommandOptions_"></a>

### SendAsync(string, object, bool, CommandOptions)


```csharp
Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
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
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | If the [Connection](/api/PuppeteerSharp.Cdp/Connection/) is closed. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPConnection.cs#L40)

<a id="PuppeteerSharp_ICDPConnection_SendAsync__1_System_String_System_Object_PuppeteerSharp_CommandOptions_"></a>

### `SendAsync<T>(string, object, CommandOptions)`


```csharp
Task<T> SendAsync<T>(string method, object args = null, CommandOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPConnection.cs#L26)
