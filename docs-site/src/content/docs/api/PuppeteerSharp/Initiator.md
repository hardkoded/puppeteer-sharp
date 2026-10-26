---
title: "Initiator"
description: "Information about the request initiator."
editUrl: false
prev: false
next: false
sidebar:
  label: "Initiator"
  hidden: true
---

<a id="PuppeteerSharp_Initiator"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class Initiator
```


Information about the request initiator.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L28)

## Properties

<a id="PuppeteerSharp_Initiator_ColumnNumber_"></a>
<a id="PuppeteerSharp_Initiator_ColumnNumber"></a>

### ColumnNumber


```csharp
public int? ColumnNumber { get; set; }
```


Initiator column number, set for Parser type or for Script type (when script is importing module) (0-based).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L48)

<a id="PuppeteerSharp_Initiator_LineNumber_"></a>
<a id="PuppeteerSharp_Initiator_LineNumber"></a>

### LineNumber


```csharp
public int? LineNumber { get; set; }
```


Initiator line number, set for Parser type or for Script type (when script is importing module) (0-based).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L43)

<a id="PuppeteerSharp_Initiator_RequestId_"></a>
<a id="PuppeteerSharp_Initiator_RequestId"></a>

### RequestId


```csharp
public string RequestId { get; set; }
```


Set if another request triggered this request (e.g. preflight).

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L53)

<a id="PuppeteerSharp_Initiator_Stack_"></a>
<a id="PuppeteerSharp_Initiator_Stack"></a>

### Stack


```csharp
public InitiatorStackTrace Stack { get; set; }
```


Initiator JavaScript stack trace, set for Script type only.

**Value:** [InitiatorStackTrace](/api/PuppeteerSharp/InitiatorStackTrace/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L58)

<a id="PuppeteerSharp_Initiator_Type_"></a>
<a id="PuppeteerSharp_Initiator_Type"></a>

### Type


```csharp
public InitiatorType Type { get; set; }
```


Gets or sets the type of the initiator.

**Value:** [InitiatorType](/api/PuppeteerSharp/InitiatorType/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L33)

<a id="PuppeteerSharp_Initiator_Url_"></a>
<a id="PuppeteerSharp_Initiator_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Initiator URL, set for Parser type or for Script type (when script is importing module) or for SignedExchange type.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Initiator.cs#L38)
