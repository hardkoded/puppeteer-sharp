---
title: "PageErrorEventArgs"
description: "Page error event arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "PageErrorEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_PageErrorEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class PageErrorEventArgs : EventArgs
```


Page error event arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L9)

## Constructors

<a id="PuppeteerSharp_PageErrorEventArgs__ctor_"></a>
<a id="PuppeteerSharp_PageErrorEventArgs__ctor_System_String_"></a>

### PageErrorEventArgs(string)


```csharp
public PageErrorEventArgs(string message)
```


Initializes a new instance of the [PageErrorEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L15)

<a id="PuppeteerSharp_PageErrorEventArgs__ctor_System_String_System_Object_"></a>

### PageErrorEventArgs(string, object)


```csharp
public PageErrorEventArgs(string message, object error)
```


Initializes a new instance of the [PageErrorEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `error` | [object](https://learn.microsoft.com/dotnet/api/system.object) | Raw error value. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L26)

<a id="PuppeteerSharp_PageErrorEventArgs__ctor_System_String_System_Object_System_Collections_Generic_IReadOnlyList_PuppeteerSharp_ConsoleMessageLocation__"></a>

### `PageErrorEventArgs(string, object, IReadOnlyList<ConsoleMessageLocation>)`


```csharp
public PageErrorEventArgs(string message, object error, IReadOnlyList<ConsoleMessageLocation> stackTrace)
```


Initializes a new instance of the [PageErrorEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `error` | [object](https://learn.microsoft.com/dotnet/api/system.object) | Raw error value. |
| `stackTrace` | [IReadOnlyList&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1) | Stack trace, innermost frame first. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L38)

## Properties

<a id="PuppeteerSharp_PageErrorEventArgs_Error_"></a>
<a id="PuppeteerSharp_PageErrorEventArgs_Error"></a>

### Error


```csharp
public object Error { get; }
```


Gets the raw error value. This can be an [string](https://learn.microsoft.com/dotnet/api/system.string) for standard errors,
or any other type (including `null`) when the page throws a primitive value.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L55)

<a id="PuppeteerSharp_PageErrorEventArgs_Message_"></a>
<a id="PuppeteerSharp_PageErrorEventArgs_Message"></a>

### Message


```csharp
public string Message { get; set; }
```


Error Message.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The message.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L49)

<a id="PuppeteerSharp_PageErrorEventArgs_StackTrace_"></a>
<a id="PuppeteerSharp_PageErrorEventArgs_StackTrace"></a>

### StackTrace


```csharp
public IReadOnlyList<ConsoleMessageLocation> StackTrace { get; }
```


Gets the stack trace of the error, innermost frame first. Empty when the browser did not report one.

**Value:** [IReadOnlyList&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageErrorEventArgs.cs#L60)
