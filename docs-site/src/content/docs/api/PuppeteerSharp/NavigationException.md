---
title: "NavigationException"
description: "Exception thrown when a IPage fails to navigate an URL."
editUrl: false
prev: false
next: false
sidebar:
  label: "NavigationException"
  hidden: true
---

<a id="PuppeteerSharp_NavigationException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class NavigationException : PuppeteerException, ISerializable
```


Exception thrown when a [IPage](/api/PuppeteerSharp/IPage/) fails to navigate an URL.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L8)

## Constructors

<a id="PuppeteerSharp_NavigationException__ctor_"></a>
<a id="PuppeteerSharp_NavigationException__ctor"></a>

### NavigationException()


```csharp
public NavigationException()
```


Initializes a new instance of the [NavigationException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L14)

<a id="PuppeteerSharp_NavigationException__ctor_System_String_"></a>

### NavigationException(string)


```csharp
public NavigationException(string message)
```


Initializes a new instance of the [NavigationException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L22)

<a id="PuppeteerSharp_NavigationException__ctor_System_String_System_Exception_"></a>

### NavigationException(string, Exception)


```csharp
public NavigationException(string message, Exception innerException)
```


Initializes a new instance of the [NavigationException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L42)

<a id="PuppeteerSharp_NavigationException__ctor_System_String_System_String_System_Exception_"></a>

### NavigationException(string, string, Exception)


```csharp
public NavigationException(string message, string url, Exception innerException = null)
```


Initializes a new instance of the [NavigationException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `url` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Url. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L32)

## Properties

<a id="PuppeteerSharp_NavigationException_Message_"></a>
<a id="PuppeteerSharp_NavigationException_Message"></a>

### Message


```csharp
public override string Message { get; }
```


Gets a message that describes the current exception.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The error message that explains the reason for the exception, or an empty string ("").

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L46)

<a id="PuppeteerSharp_NavigationException_Url_"></a>
<a id="PuppeteerSharp_NavigationException_Url"></a>

### Url


```csharp
public string Url { get; }
```


Url that caused the exception.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The URL.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/NavigationException.cs#L63)
