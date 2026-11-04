---
title: "ConsoleEventArgs"
description: "Console data."
editUrl: false
prev: false
next: false
sidebar:
  label: "ConsoleEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_ConsoleEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ConsoleEventArgs : EventArgs
```


[Console](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Console) data.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_ConsoleEventArgs__ctor_"></a>
<a id="PuppeteerSharp_ConsoleEventArgs__ctor_PuppeteerSharp_ConsoleMessage_"></a>

### ConsoleEventArgs(ConsoleMessage)


```csharp
public ConsoleEventArgs(ConsoleMessage message)
```


Initializes a new instance of the [ConsoleEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [ConsoleMessage](/api/PuppeteerSharp/ConsoleMessage/) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_ConsoleEventArgs_Message_"></a>
<a id="PuppeteerSharp_ConsoleEventArgs_Message"></a>

### Message


```csharp
public ConsoleMessage Message { get; }
```


Gets the message.

**Value:** [ConsoleMessage](/api/PuppeteerSharp/ConsoleMessage/) — The message.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleEventArgs.cs#L20)
