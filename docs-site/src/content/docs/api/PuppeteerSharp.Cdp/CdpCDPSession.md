---
title: "CdpCDPSession"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "CdpCDPSession"
  hidden: true
---

<a id="PuppeteerSharp_Cdp_CdpCDPSession"></a>

Namespace: [PuppeteerSharp.Cdp](/api/PuppeteerSharp.Cdp/)


```csharp
public class CdpCDPSession : CDPSession, ICDPSession, ICDPConnection
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [CDPSession](/api/PuppeteerSharp/CDPSession/)

**Implements:** [ICDPSession](/api/PuppeteerSharp/ICDPSession/), [ICDPConnection](/api/PuppeteerSharp/ICDPConnection/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpCDPSession.cs#L38)

## Properties

<a id="PuppeteerSharp_Cdp_CdpCDPSession_Detached_"></a>
<a id="PuppeteerSharp_Cdp_CdpCDPSession_Detached"></a>

### Detached


```csharp
public override bool Detached { get; }
```


True if the session has been detached, false otherwise.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpCDPSession.cs#L54)

## Methods

<a id="PuppeteerSharp_Cdp_CdpCDPSession_Close_"></a>
<a id="PuppeteerSharp_Cdp_CdpCDPSession_Close_System_String_"></a>

### Close(string)


```csharp
public override void Close(string closeReason)
```


Closes the session.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `closeReason` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The close reason. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpCDPSession.cs#L132)

<a id="PuppeteerSharp_Cdp_CdpCDPSession_DetachAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpCDPSession_DetachAsync"></a>

### DetachAsync()


```csharp
public override Task DetachAsync()
```


Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that when awaited detaches from the session target.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | If the [Connection](/api/PuppeteerSharp.Cdp/Connection/) is closed. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpCDPSession.cs#L65)

<a id="PuppeteerSharp_Cdp_CdpCDPSession_SendAsync_"></a>
<a id="PuppeteerSharp_Cdp_CdpCDPSession_SendAsync_System_String_System_Object_System_Boolean_PuppeteerSharp_CommandOptions_"></a>

### SendAsync(string, object, bool, CommandOptions)


```csharp
public override Task<JsonElement?> SendAsync(string method, object args = null, bool waitForCallback = true, CommandOptions options = null)
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

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Cdp/CdpCDPSession.cs#L77)
