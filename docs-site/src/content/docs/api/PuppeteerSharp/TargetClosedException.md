---
title: "TargetClosedException"
description: "Exception thrown by the Connection when it detects that the target was closed."
editUrl: false
prev: false
next: false
sidebar:
  label: "TargetClosedException"
  hidden: true
---

<a id="PuppeteerSharp_TargetClosedException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class TargetClosedException : PuppeteerException, ISerializable
```


Exception thrown by the [Connection](/api/PuppeteerSharp.Cdp/Connection/) when it detects that the target was closed.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L8)

## Constructors

<a id="PuppeteerSharp_TargetClosedException__ctor_"></a>
<a id="PuppeteerSharp_TargetClosedException__ctor"></a>

### TargetClosedException()


```csharp
public TargetClosedException()
```


Initializes a new instance of the [TargetClosedException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L14)

<a id="PuppeteerSharp_TargetClosedException__ctor_System_String_"></a>

### TargetClosedException(string)


```csharp
public TargetClosedException(string message)
```


Initializes a new instance of the [TargetClosedException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L22)

<a id="PuppeteerSharp_TargetClosedException__ctor_System_String_System_Exception_"></a>

### TargetClosedException(string, Exception)


```csharp
public TargetClosedException(string message, Exception innerException)
```


Initializes a new instance of the [TargetClosedException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L31)

<a id="PuppeteerSharp_TargetClosedException__ctor_System_String_System_String_"></a>

### TargetClosedException(string, string)


```csharp
public TargetClosedException(string message, string closeReason)
```


Initializes a new instance of the [TargetClosedException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `closeReason` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Close reason. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L40)

## Properties

<a id="PuppeteerSharp_TargetClosedException_CloseReason_"></a>
<a id="PuppeteerSharp_TargetClosedException_CloseReason"></a>

### CloseReason


```csharp
public string CloseReason { get; }
```


Close Reason.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The close reason.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/TargetClosedException.cs#L47)
