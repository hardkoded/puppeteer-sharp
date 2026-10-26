---
title: "CDPSession"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CDPSession"
  hidden: true
---

<a id="PuppeteerSharp_CDPSession"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public abstract class CDPSession : ICDPSession, ICDPConnection
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/), [ICDPConnection](/api/PuppeteerSharp/ICDPConnection/)

**Derived:** [BidiCdpSession](/api/PuppeteerSharp.Bidi/BidiCdpSession/), [CdpCDPSession](/api/PuppeteerSharp.Cdp/CdpCDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L13)

## Properties

<a id="PuppeteerSharp_CDPSession_CloseReason_"></a>
<a id="PuppeteerSharp_CDPSession_CloseReason"></a>

### CloseReason


```csharp
public string CloseReason { get; protected set; }
```


Close reason if the session has been closed.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L62)

<a id="PuppeteerSharp_CDPSession_Detached_"></a>
<a id="PuppeteerSharp_CDPSession_Detached"></a>

### Detached


```csharp
public abstract bool Detached { get; }
```


True if the session has been detached, false otherwise.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L65)

<a id="PuppeteerSharp_CDPSession_Id_"></a>
<a id="PuppeteerSharp_CDPSession_Id"></a>

### Id


```csharp
public string Id { get; init; }
```


Gets the session identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The session identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L59)

<a id="PuppeteerSharp_CDPSession_LoggerFactory_"></a>
<a id="PuppeteerSharp_CDPSession_LoggerFactory"></a>

### LoggerFactory


```csharp
public virtual ILoggerFactory LoggerFactory { get; }
```


Gets the logger factory.

**Value:** [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) — The logger factory.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L68)

## Events

<a id="PuppeteerSharp_CDPSession_Disconnected"></a>

### Disconnected


```csharp
public event EventHandler Disconnected
```


Occurs when the connection is closed.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L46)

<a id="PuppeteerSharp_CDPSession_MessageReceived"></a>

### MessageReceived


```csharp
public event EventHandler<MessageEventArgs> MessageReceived
```


Occurs when message received from Chromium.

**Value:** [EventHandler&lt;MessageEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L21)

<a id="PuppeteerSharp_CDPSession_SessionAttached"></a>

### SessionAttached


```csharp
public event EventHandler<SessionEventArgs> SessionAttached
```


Occurs when the session is attached to the target.

**Value:** [EventHandler&lt;SessionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L49)

<a id="PuppeteerSharp_CDPSession_SessionDetached"></a>

### SessionDetached


```csharp
public event EventHandler<SessionEventArgs> SessionDetached
```


Occurs when the session is detached from the target.

**Value:** [EventHandler&lt;SessionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L52)

## Methods

<a id="PuppeteerSharp_CDPSession_Close_"></a>
<a id="PuppeteerSharp_CDPSession_Close_System_String_"></a>

### Close(string)


```csharp
public abstract void Close(string closeReason)
```


Closes the session.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `closeReason` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The close reason. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L102)

<a id="PuppeteerSharp_CDPSession_DetachAsync_"></a>
<a id="PuppeteerSharp_CDPSession_DetachAsync"></a>

### DetachAsync()


```csharp
public abstract Task DetachAsync()
```


Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that when awaited detaches from the session target.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | If the [Connection](/api/PuppeteerSharp.Cdp/Connection/) is closed. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L99)

<a id="PuppeteerSharp_CDPSession_OnDisconnected_"></a>
<a id="PuppeteerSharp_CDPSession_OnDisconnected"></a>

### OnDisconnected()


```csharp
protected void OnDisconnected()
```


Emits [Disconnected](#PuppeteerSharp_CDPSession_Disconnected) event.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L177)

<a id="PuppeteerSharp_CDPSession_OnMessageReceived_"></a>
<a id="PuppeteerSharp_CDPSession_OnMessageReceived_PuppeteerSharp_MessageEventArgs_"></a>

### OnMessageReceived(MessageEventArgs)


```csharp
protected void OnMessageReceived(MessageEventArgs e)
```


Emits [MessageReceived](#PuppeteerSharp_CDPSession_MessageReceived) event.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `e` | [MessageEventArgs](/api/PuppeteerSharp/MessageEventArgs/) | Event arguments. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L157)

<a id="PuppeteerSharp_CDPSession_SendAsync_"></a>
<a id="PuppeteerSharp_CDPSession_SendAsync_System_String_System_Object_System_Boolean_PuppeteerSharp_CommandOptions_"></a>

### SendAsync(string, object, bool, CommandOptions)


```csharp
public abstract Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L96)

<a id="PuppeteerSharp_CDPSession_SendAsync__1_System_String_System_Object_PuppeteerSharp_CommandOptions_"></a>

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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/CDPSession.cs#L88)
