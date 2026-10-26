---
title: "WaitTaskTimeoutException"
description: "Timeout exception that might be thrown by WaitFor methods in Frame."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitTaskTimeoutException"
  hidden: true
---

<a id="PuppeteerSharp_WaitTaskTimeoutException"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
[Serializable]
public class WaitTaskTimeoutException : PuppeteerException, ISerializable
```


Timeout exception that might be thrown by `WaitFor` methods in [Frame](/api/PuppeteerSharp/Frame/).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [Exception](https://learn.microsoft.com/dotnet/api/system.exception) → [PuppeteerException](/api/PuppeteerSharp/PuppeteerException/)

**Implements:** [ISerializable](https://learn.microsoft.com/dotnet/api/system.runtime.serialization.iserializable)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L8)

## Constructors

<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor_"></a>
<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor"></a>

### WaitTaskTimeoutException()


```csharp
public WaitTaskTimeoutException()
```


Initializes a new instance of the [WaitTaskTimeoutException](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L14)

<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor_System_Int32_"></a>

### WaitTaskTimeoutException(int)


```csharp
public WaitTaskTimeoutException(int timeout)
```


Initializes a new instance of the [WaitTaskTimeoutException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Timeout. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L30)

<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor_System_Int32_System_String_"></a>

### WaitTaskTimeoutException(int, string)


```csharp
public WaitTaskTimeoutException(int timeout, string elementType)
```


Initializes a new instance of the [WaitTaskTimeoutException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `timeout` | [int](https://learn.microsoft.com/dotnet/api/system.int32) | Timeout. |
| `elementType` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Element type. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L40)

<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor_System_String_"></a>

### WaitTaskTimeoutException(string)


```csharp
public WaitTaskTimeoutException(string message)
```


Initializes a new instance of the [WaitTaskTimeoutException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L22)

<a id="PuppeteerSharp_WaitTaskTimeoutException__ctor_System_String_System_Exception_"></a>

### WaitTaskTimeoutException(string, Exception)


```csharp
public WaitTaskTimeoutException(string message, Exception innerException)
```


Initializes a new instance of the [WaitTaskTimeoutException](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `message` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Message. |
| `innerException` | [Exception](https://learn.microsoft.com/dotnet/api/system.exception) | Inner exception. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L51)

## Properties

<a id="PuppeteerSharp_WaitTaskTimeoutException_ElementType_"></a>
<a id="PuppeteerSharp_WaitTaskTimeoutException_ElementType"></a>

### ElementType


```csharp
public string ElementType { get; }
```


Element type the WaitTask was waiting for.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The element.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L65)

<a id="PuppeteerSharp_WaitTaskTimeoutException_Timeout_"></a>
<a id="PuppeteerSharp_WaitTaskTimeoutException_Timeout"></a>

### Timeout


```csharp
public int Timeout { get; }
```


Timeout that caused the exception.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — The timeout.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitTaskTimeoutException.cs#L59)
