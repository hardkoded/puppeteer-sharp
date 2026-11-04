---
title: "MessageEventArgs"
description: "MessageReceived arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "MessageEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_MessageEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class MessageEventArgs : EventArgs
```


[MessageReceived](/api/PuppeteerSharp/CDPSession/#PuppeteerSharp_CDPSession_MessageReceived) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageEventArgs.cs#L9)

## Properties

<a id="PuppeteerSharp_MessageEventArgs_MessageData_"></a>
<a id="PuppeteerSharp_MessageEventArgs_MessageData"></a>

### MessageData


```csharp
public JsonElement MessageData { get; }
```


Gets the message data.

**Value:** [JsonElement](https://learn.microsoft.com/dotnet/api/system.text.json.jsonelement) — The message data.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageEventArgs.cs#L21)

<a id="PuppeteerSharp_MessageEventArgs_MessageID_"></a>
<a id="PuppeteerSharp_MessageEventArgs_MessageID"></a>

### MessageID


```csharp
public string MessageID { get; }
```


Gets the message identifier.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The message identifier.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageEventArgs.cs#L15)
