---
title: "ErrorEventArgs"
description: "Error arguments."
editUrl: false
prev: false
next: false
sidebar:
  label: "ErrorEventArgs"
  hidden: true
---

<a id="PuppeteerSharp_ErrorEventArgs"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ErrorEventArgs : EventArgs
```


[Error](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_Error) arguments.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [EventArgs](https://learn.microsoft.com/dotnet/api/system.eventargs)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ErrorEventArgs.cs#L8)

## Constructors

<a id="PuppeteerSharp_ErrorEventArgs__ctor_"></a>
<a id="PuppeteerSharp_ErrorEventArgs__ctor_System_String_"></a>

### ErrorEventArgs(string)


```csharp
public ErrorEventArgs(string error)
```


Initializes a new instance of the [ErrorEventArgs](./) class.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `error` | [string](https://learn.microsoft.com/dotnet/api/system.string) | Error. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ErrorEventArgs.cs#L14)

## Properties

<a id="PuppeteerSharp_ErrorEventArgs_Error_"></a>
<a id="PuppeteerSharp_ErrorEventArgs_Error"></a>

### Error


```csharp
public string Error { get; }
```


Gets the error.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string) — The error.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ErrorEventArgs.cs#L23)
