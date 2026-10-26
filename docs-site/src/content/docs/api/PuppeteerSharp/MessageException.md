---
title: "MessageException"
description: "Exception thrown by. CommandOptions)"
editUrl: false
prev: false
next: false
sidebar:
  label: "MessageException"
  hidden: true
---

<a id="PuppeteerSharp_MessageException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class MessageException : PuppeteerException, ISerializable
```


Exception thrown by. `CommandOptions)`

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageException.cs#L9)

## Constructors

<a id="PuppeteerSharp_MessageException__ctor_"></a>
<a id="PuppeteerSharp_MessageException__ctor"></a>

### MessageException()


```csharp
public MessageException()
```


Initializes a new instance of the [MessageException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageException.cs#L15)

<a id="PuppeteerSharp_MessageException__ctor_System_String_"></a>

### MessageException(string)


```csharp
public MessageException(string message)
```


Initializes a new instance of the [MessageException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageException.cs#L23)

<a id="PuppeteerSharp_MessageException__ctor_System_String_System_Exception_"></a>

### MessageException(string, Exception)


```csharp
public MessageException(string message, Exception innerException)
```


Initializes a new instance of the [MessageException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/MessageException.cs#L32)
