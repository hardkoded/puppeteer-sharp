---
title: "EvaluationFailedException"
description: "Exception thrown by Object)."
editUrl: false
prev: false
next: false
sidebar:
  label: "EvaluationFailedException"
  hidden: true
---

<a id="PuppeteerSharp_EvaluationFailedException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class EvaluationFailedException : PuppeteerException, ISerializable
```


Exception thrown by `Object)`.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EvaluationFailedException.cs#L8)

## Constructors

<a id="PuppeteerSharp_EvaluationFailedException__ctor_"></a>
<a id="PuppeteerSharp_EvaluationFailedException__ctor"></a>

### EvaluationFailedException()


```csharp
public EvaluationFailedException()
```


Initializes a new instance of the [EvaluationFailedException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EvaluationFailedException.cs#L14)

<a id="PuppeteerSharp_EvaluationFailedException__ctor_System_String_"></a>

### EvaluationFailedException(string)


```csharp
public EvaluationFailedException(string message)
```


Initializes a new instance of the [EvaluationFailedException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EvaluationFailedException.cs#L22)

<a id="PuppeteerSharp_EvaluationFailedException__ctor_System_String_System_Exception_"></a>

### EvaluationFailedException(string, Exception)


```csharp
public EvaluationFailedException(string message, Exception innerException)
```


Initializes a new instance of the [EvaluationFailedException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/EvaluationFailedException.cs#L31)
