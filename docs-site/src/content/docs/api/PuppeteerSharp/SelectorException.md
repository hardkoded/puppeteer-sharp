---
title: "SelectorException"
description: "Exception thrown when an element selector returns null."
editUrl: false
prev: false
next: false
sidebar:
  label: "SelectorException"
  hidden: true
---

<a id="PuppeteerSharp_SelectorException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class SelectorException : PuppeteerException, ISerializable
```


Exception thrown when an element selector returns null.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L15)

## Constructors

<a id="PuppeteerSharp_SelectorException__ctor_"></a>
<a id="PuppeteerSharp_SelectorException__ctor"></a>

### SelectorException()


```csharp
public SelectorException()
```


Initializes a new instance of the [SelectorException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L21)

<a id="PuppeteerSharp_SelectorException__ctor_System_String_"></a>

### SelectorException(string)


```csharp
public SelectorException(string message)
```


Initializes a new instance of the [SelectorException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L29)

<a id="PuppeteerSharp_SelectorException__ctor_System_String_System_Exception_"></a>

### SelectorException(string, Exception)


```csharp
public SelectorException(string message, Exception innerException)
```


Initializes a new instance of the [SelectorException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L48)

<a id="PuppeteerSharp_SelectorException__ctor_System_String_System_String_"></a>

### SelectorException(string, string)


```csharp
public SelectorException(string message, string selector)
```


Initializes a new instance of the [SelectorException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `selector` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Selector. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L38)

## Properties

<a id="PuppeteerSharp_SelectorException_Selector_"></a>
<a id="PuppeteerSharp_SelectorException_Selector"></a>

### Selector


```csharp
public string Selector { get; }
```


Gets the selector.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The selector.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/SelectorException.cs#L56)
