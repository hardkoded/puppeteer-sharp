---
title: "Offset"
description: "Offset used in conjunction with Offset%7d)."
editUrl: false
prev: false
next: false
sidebar:
  label: "Offset"
  hidden: true
---

<a id="PuppeteerSharp_Offset"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public struct Offset
```


Offset used in conjunction with `Offset%7d)`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Offset.cs#L6)

## Constructors

<a id="PuppeteerSharp_Offset__ctor_"></a>
<a id="PuppeteerSharp_Offset__ctor_System_Decimal_System_Decimal_"></a>

### Offset(decimal, decimal)


```csharp
public Offset(decimal x, decimal y)
```


Initializes a new instance of the [Offset](./) struct.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `x` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | x-offset for the clickable point relative to the top-left corner of the border box. |
| `y` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | y-offset for the clickable point relative to the top-left corner of the border box. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Offset.cs#L13)

## Properties

<a id="PuppeteerSharp_Offset_X_"></a>
<a id="PuppeteerSharp_Offset_X"></a>

### X


```csharp
public decimal X { readonly get; set; }
```


x-offset for the clickable point relative to the top-left corner of the border box.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Offset.cs#L22)

<a id="PuppeteerSharp_Offset_Y_"></a>
<a id="PuppeteerSharp_Offset_Y"></a>

### Y


```csharp
public decimal Y { readonly get; set; }
```


y-offset for the clickable point relative to the top-left corner of the border box.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Offset.cs#L27)
