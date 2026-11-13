---
title: "RemoteException"
description: "A protocol-agnostic description of an uncaught exception thrown in a page or worker. Dispatched via the ExceptionThrown and ExceptionThrown events."
editUrl: false
prev: false
next: false
sidebar:
  label: "RemoteException"
  hidden: true
---

<a id="PuppeteerSharp_RemoteException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class RemoteException
```


A protocol-agnostic description of an uncaught exception thrown in a page or worker.
Dispatched via the [ExceptionThrown](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ExceptionThrown) and [ExceptionThrown](/api/PuppeteerSharp/WebWorker/#PuppeteerSharp_WebWorker_ExceptionThrown) events.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

Unlike the [PageError](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_PageError) event, [RemoteException](./) does not
reconstruct an error value. When the protocol supports it (CDP), it exposes the live remote
exception value through [Handle](#PuppeteerSharp_RemoteException_Handle). Each call frame of [StackTrace](#PuppeteerSharp_RemoteException_StackTrace) is reported as a
[ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/) (URL, line number and column number).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L15)

## Properties

<a id="PuppeteerSharp_RemoteException_Handle_"></a>
<a id="PuppeteerSharp_RemoteException_Handle"></a>

### Handle


```csharp
public IJSHandle Handle { get; }
```


Gets the live remote exception value, if available. `null` for thrown primitives other than
symbols, for exceptions thrown in other isolated worlds, and for exceptions thrown by code that
Puppeteer injected.

**Value:** [IJSHandle](/api/PuppeteerSharp/IJSHandle/)

The handle is disposed automatically right after the event has been dispatched. To keep using it
beyond the synchronous part of your handler, call [Move()](/api/PuppeteerSharp/IJSHandle/#PuppeteerSharp_IJSHandle_Move) synchronously inside
the handler and dispose of the handle yourself once you are done with it.
The handle is shared by all handlers of the event.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L69)

<a id="PuppeteerSharp_RemoteException_Location_"></a>
<a id="PuppeteerSharp_RemoteException_Location"></a>

### Location


```csharp
public ConsoleMessageLocation Location { get; }
```


Gets the primary location where the exception occurred.
For runtime errors this is the first element of [StackTrace](#PuppeteerSharp_RemoteException_StackTrace). For compilation and
syntax errors (where [StackTrace](#PuppeteerSharp_RemoteException_StackTrace) is empty), it falls back to the top-level
coordinates reported for the exception.

**Value:** [ConsoleMessageLocation](/api/PuppeteerSharp/ConsoleMessageLocation/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L56)

<a id="PuppeteerSharp_RemoteException_Name_"></a>
<a id="PuppeteerSharp_RemoteException_Name"></a>

### Name


```csharp
public string Name { get; }
```


Gets the name of the exception, for example `Error`, `TypeError`, `SyntaxError` or `AbortError`.
Empty string if the thrown value is not an error, for example a primitive, a plain object or a symbol.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L42)

<a id="PuppeteerSharp_RemoteException_StackTrace_"></a>
<a id="PuppeteerSharp_RemoteException_StackTrace"></a>

### StackTrace


```csharp
public IReadOnlyList<ConsoleMessageLocation> StackTrace { get; }
```


Gets the call frames on the stack at the time the exception was thrown, innermost first.
Empty for compilation and syntax errors.

**Value:** [IReadOnlyList&lt;ConsoleMessageLocation&gt;](https://learn.microsoft.com/dotnet/api/system.collections.generic.ireadonlylist-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L48)

<a id="PuppeteerSharp_RemoteException_Text_"></a>
<a id="PuppeteerSharp_RemoteException_Text"></a>

### Text


```csharp
public string Text { get; }
```


Gets the text of the exception as rendered by the browser, for example `Error: boom`.
For errors this is `${name}: ${message}` (or just the name if the message is empty), without the stack.
For other thrown values it is their description, for example `Object` for a plain object.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteException.cs#L36)
