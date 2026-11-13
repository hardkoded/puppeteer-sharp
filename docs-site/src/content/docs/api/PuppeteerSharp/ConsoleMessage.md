---
title: "ConsoleMessage"
description: "ConsoleMessage is part of ConsoleEventArgs used by Console."
editUrl: false
prev: false
next: false
sidebar:
  label: "ConsoleMessage"
  hidden: true
---

<a id="PuppeteerSharp_ConsoleMessage"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ConsoleMessage
```


ConsoleMessage is part of [ConsoleEventArgs](/api/PuppeteerSharp/ConsoleEventArgs/) used by [Console](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Console).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L8)

## Constructors

<a id="PuppeteerSharp_ConsoleMessage__ctor_"></a>
<a id="PuppeteerSharp_ConsoleMessage__ctor_PuppeteerSharp_ConsoleType_System_String_System_Collections_Generic_IList_PuppeteerSharp_IJSHandle__PuppeteerSharp_ConsoleMessageLocation_System_Collections_Generic_IList_PuppeteerSharp_ConsoleMessageLocation__"></a>

### `ConsoleMessage(ConsoleType, string, IList<IJSHandle>, ConsoleMessageLocation, IList<ConsoleMessageLocation>)`


```csharp
public ConsoleMessage(ConsoleType type, string text, IList<IJSHandle> args, ConsoleMessageLocation location = null, IList<ConsoleMessageLocation> stackTrace = null)
```


Initializes a new instance of the [ConsoleMessage](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `type` | [ConsoleType](/api/PuppeteerSharp/ConsoleType/) | Type. |
| `text` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Text. |
| `args` | [IList&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1) | Arguments. |
| `location` | [ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/) | Message location. |
| `stackTrace` | [IList&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1) | Stack trace. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L18)

## Properties

<a id="PuppeteerSharp_ConsoleMessage_Args_"></a>
<a id="PuppeteerSharp_ConsoleMessage_Args"></a>

### Args


```csharp
public IList<IJSHandle> Args { get; }
```


Gets the arguments.

**Value:** [IList&lt;IJSHandle&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1) — The arguments.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L43)

<a id="PuppeteerSharp_ConsoleMessage_Location_"></a>
<a id="PuppeteerSharp_ConsoleMessage_Location"></a>

### Location


```csharp
public ConsoleMessageLocation Location { get; }
```


Gets the location.

**Value:** [ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L48)

<a id="PuppeteerSharp_ConsoleMessage_StackTrace_"></a>
<a id="PuppeteerSharp_ConsoleMessage_StackTrace"></a>

### StackTrace


```csharp
public IList<ConsoleMessageLocation> StackTrace { get; }
```


Gets the stack trace.

**Value:** [IList&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ilist-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L53)

<a id="PuppeteerSharp_ConsoleMessage_Text_"></a>
<a id="PuppeteerSharp_ConsoleMessage_Text"></a>

### Text


```csharp
public string Text { get; }
```


Gets the console text.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The text.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L37)

<a id="PuppeteerSharp_ConsoleMessage_Type_"></a>
<a id="PuppeteerSharp_ConsoleMessage_Type"></a>

### Type


```csharp
public ConsoleType Type { get; }
```


Gets the ConsoleMessage type.

**Value:** [ConsoleType](/api/PuppeteerSharp/ConsoleType/) — ConsoleMessageType.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ConsoleMessage.cs#L31)
