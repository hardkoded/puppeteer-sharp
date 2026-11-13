---
title: "RemoteExceptionEventArgs"
description: "Event arguments for uncaught exceptions thrown in a page or worker."
editUrl: false
prev: false
next: false
sidebar:
  label: "RemoteExceptionEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_RemoteExceptionEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class RemoteExceptionEventArgs : EventArgs
```


Event arguments for uncaught exceptions thrown in a page or worker.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteExceptionEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_RemoteExceptionEventArgs__ctor_"></a>
<a id="PuppeteerSharp_RemoteExceptionEventArgs__ctor_PuppeteerSharp_RemoteException_"></a>

### RemoteExceptionEventArgs(RemoteException)


```csharp
public RemoteExceptionEventArgs(RemoteException exception)
```


Initializes a new instance of the [RemoteExceptionEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `exception` | [RemoteException](/api/PuppeteerSharp/RemoteException/) | The exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteExceptionEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_RemoteExceptionEventArgs_Exception_"></a>
<a id="PuppeteerSharp_RemoteExceptionEventArgs_Exception"></a>

### Exception


```csharp
public RemoteException Exception { get; }
```


Gets the exception.

**Value:** [RemoteException](/api/PuppeteerSharp/RemoteException/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RemoteExceptionEventArgs.cs#L19)
