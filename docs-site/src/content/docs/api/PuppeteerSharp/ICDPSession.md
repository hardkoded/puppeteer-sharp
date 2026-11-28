---
title: "ICDPSession"
description: "The CDPSession instances are used to talk raw Chrome Devtools Protocol: Protocol methods can be called with ICDPConnection.SendAsync() method. Protocol event..."
editUrl: false
prev: false
next: false
sidebar:
  label: "ICDPSession"
  hidden: true
---

<a id="PuppeteerSharp_ICDPSession"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public interface ICDPSession : ICDPConnection
```


The CDPSession instances are used to talk raw Chrome Devtools Protocol:
 * Protocol methods can be called with `ICDPConnection.SendAsync()` method.
 * Protocol events, using the [MessageReceived](/api/PuppeteerSharp/ICDPConnection/#PuppeteerSharp_ICDPConnection_MessageReceived) event.

Documentation on DevTools Protocol can be found here: [https://chromedevtools.github.io/devtools-protocol/](https://chromedevtools.github.io/devtools-protocol/).


```csharp
var client = await Page.Target.CreateCDPSessionAsync();
await client.SendAsync("Animation.enable");
client.MessageReceived += (sender, e) =>
{
     if (e.MessageID == "Animation.animationCreated")
     {
         Console.WriteLine("Animation created!");
     }
};
JObject response = await client.SendAsync("Animation.getPlaybackRate");
Console.WriteLine("playback rate is " + response.playbackRate);
await client.SendAsync("Animation.setPlaybackRate", new
{
    playbackRate = Convert.ToInt32(response.playbackRate / 2)
});
```

**Derived:** [BidiCdpSession](/api/PuppeteerSharp.Bidi/BidiCdpSession/), [CDPSession](/api/PuppeteerSharp/CDPSession/), [CdpCDPSession](/api/PuppeteerSharp.Cdp/CdpCDPSession/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L34)

## Properties

<a id="PuppeteerSharp_ICDPSession_CloseReason_"></a>
<a id="PuppeteerSharp_ICDPSession_CloseReason"></a>

### CloseReason


```csharp
string CloseReason { get; }
```


Close reason if the session has been closed.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L66)

<a id="PuppeteerSharp_ICDPSession_Detached_"></a>
<a id="PuppeteerSharp_ICDPSession_Detached"></a>

### Detached


```csharp
bool Detached { get; }
```


True if the session has been detached, false otherwise.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L71)

<a id="PuppeteerSharp_ICDPSession_Id_"></a>
<a id="PuppeteerSharp_ICDPSession_Id"></a>

### Id


```csharp
string Id { get; }
```


Gets the session identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The session identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L61)

<a id="PuppeteerSharp_ICDPSession_LoggerFactory_"></a>
<a id="PuppeteerSharp_ICDPSession_LoggerFactory"></a>

### LoggerFactory


```csharp
ILoggerFactory LoggerFactory { get; }
```


Gets the logger factory.

**Value:** [ILoggerFactory](https://learn.microsoft.com/dotnet/api/microsoft.extensions.logging.iloggerfactory) — The logger factory.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L55)

## Events

<a id="PuppeteerSharp_ICDPSession_Disconnected"></a>

### Disconnected


```csharp
event EventHandler Disconnected
```


Occurs when the connection is closed.

**Value:** [EventHandler](https://learn.microsoft.com/dotnet/api/system.eventhandler)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L39)

<a id="PuppeteerSharp_ICDPSession_SessionAttached"></a>

### SessionAttached


```csharp
event EventHandler<SessionEventArgs> SessionAttached
```


Occurs when the session is attached to the target.

**Value:** [EventHandler&lt;SessionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L44)

<a id="PuppeteerSharp_ICDPSession_SessionDetached"></a>

### SessionDetached


```csharp
event EventHandler<SessionEventArgs> SessionDetached
```


Occurs when the session is detached from the target.

**Value:** [EventHandler&lt;SessionEventArgs&gt;](https://learn.microsoft.com/dotnet/api/system.eventhandler-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L49)

## Methods

<a id="PuppeteerSharp_ICDPSession_Close_"></a>
<a id="PuppeteerSharp_ICDPSession_Close_System_String_"></a>

### Close(string)


```csharp
void Close(string closeReason)
```


Closes the session.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `closeReason` | [string](https://learn.microsoft.com/dotnet/api/system.string) | The close reason. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L84)

<a id="PuppeteerSharp_ICDPSession_DetachAsync_"></a>
<a id="PuppeteerSharp_ICDPSession_DetachAsync"></a>

### DetachAsync()


```csharp
Task DetachAsync()
```


Detaches session from target. Once detached, session won't emit any events and can't be used to send messages.

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A Task that when awaited detaches from the session target.

**Exceptions**

| Type | Condition |
| --- | --- |
| [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/) | If the [Connection](/api/PuppeteerSharp.Cdp/Connection/) is closed. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ICDPSession.cs#L78)
