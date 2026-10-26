---
title: "PuppeteerException"
description: "Base exception used to identify any exception thrown by PuppeteerSharp."
editUrl: false
prev: false
next: false
sidebar:
  label: "PuppeteerException"
  hidden: true
---

<a id="PuppeteerSharp_PuppeteerException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class PuppeteerException : Exception, ISerializable
```


Base exception used to identify any exception thrown by PuppeteerSharp.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

**Derived:** [EvaluationFailedException](/api/PuppeteerSharp/EvaluationFailedException/), [MessageException](/api/PuppeteerSharp/MessageException/), [NavigationException](/api/PuppeteerSharp/NavigationException/), [SelectorException](/api/PuppeteerSharp/SelectorException/), [TargetClosedException](/api/PuppeteerSharp/TargetClosedException/), [WaitTaskTimeoutException](/api/PuppeteerSharp/WaitTaskTimeoutException/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerException.cs#L8)

## Constructors

<a id="PuppeteerSharp_PuppeteerException__ctor_"></a>
<a id="PuppeteerSharp_PuppeteerException__ctor"></a>

### PuppeteerException()


```csharp
public PuppeteerException()
```


Initializes a new instance of the [PuppeteerException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerException.cs#L14)

<a id="PuppeteerSharp_PuppeteerException__ctor_System_String_"></a>

### PuppeteerException(string)


```csharp
public PuppeteerException(string message)
```


Initializes a new instance of the [PuppeteerException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerException.cs#L22)

<a id="PuppeteerSharp_PuppeteerException__ctor_System_String_System_Exception_"></a>

### PuppeteerException(string, Exception)


```csharp
public PuppeteerException(string message, Exception innerException)
```


Initializes a new instance of the [PuppeteerException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PuppeteerException.cs#L31)
