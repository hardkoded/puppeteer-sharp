---
title: "ProcessException"
description: "process exception thrown by Launcher."
editUrl: false
prev: false
next: false
sidebar:
  label: "ProcessException"
  hidden: true
---

<a id="PuppeteerSharp_ProcessException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class ProcessException : Exception, ISerializable
```


process exception thrown by [Launcher](/api/PuppeteerSharp/Launcher/).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ProcessException.cs#L8)

## Constructors

<a id="PuppeteerSharp_ProcessException__ctor_"></a>
<a id="PuppeteerSharp_ProcessException__ctor"></a>

### ProcessException()


```csharp
public ProcessException()
```


Initializes a new instance of the [ProcessException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ProcessException.cs#L14)

<a id="PuppeteerSharp_ProcessException__ctor_System_String_"></a>

### ProcessException(string)


```csharp
public ProcessException(string message)
```


Initializes a new instance of the [ProcessException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ProcessException.cs#L22)

<a id="PuppeteerSharp_ProcessException__ctor_System_String_System_Exception_"></a>

### ProcessException(string, Exception)


```csharp
public ProcessException(string message, Exception innerException)
```


Initializes a new instance of the [ProcessException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ProcessException.cs#L31)
