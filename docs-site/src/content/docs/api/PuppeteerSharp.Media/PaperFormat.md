---
title: "PaperFormat"
description: "Paper format."
editUrl: false
prev: false
next: false
sidebar:
  label: "PaperFormat"
  hidden: true
---

<a id="PuppeteerSharp_Media_PaperFormat"></a>

Namespace: [PuppeteerSharp.Media](/api/PuppeteerSharp.Media/)


```csharp
public record PaperFormat : IEquatable<PaperFormat>
```


Paper format.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;PaperFormat&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L7)

## Constructors

<a id="PuppeteerSharp_Media_PaperFormat__ctor_"></a>
<a id="PuppeteerSharp_Media_PaperFormat__ctor_System_Decimal_System_Decimal_"></a>

### PaperFormat(decimal, decimal)


```csharp
public PaperFormat(decimal width, decimal height)
```


Initializes a new instance of the [PaperFormat](./) class.
Page width and height in inches.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `width` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Page width in inches. |
| `height` | [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) | Page height in inches. |

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L15)

## Properties

<a id="PuppeteerSharp_Media_PaperFormat_A0_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A0"></a>

### A0


```csharp
public static PaperFormat A0 { get; }
```


A0: 33.1102 inches by 46.811 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L44)

<a id="PuppeteerSharp_Media_PaperFormat_A1_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A1"></a>

### A1


```csharp
public static PaperFormat A1 { get; }
```


A1: 23.3858 inches by 33.1102 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L49)

<a id="PuppeteerSharp_Media_PaperFormat_A2_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A2"></a>

### A2


```csharp
public static PaperFormat A2 { get; }
```


A2: 16.5354 inches by 23.3858 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L54)

<a id="PuppeteerSharp_Media_PaperFormat_A3_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A3"></a>

### A3


```csharp
public static PaperFormat A3 { get; }
```


A3: 11.6929 inches by 16.5354 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L59)

<a id="PuppeteerSharp_Media_PaperFormat_A4_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A4"></a>

### A4


```csharp
public static PaperFormat A4 { get; }
```


A4: 8.2677 inches by 11.6929 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L64)

<a id="PuppeteerSharp_Media_PaperFormat_A5_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A5"></a>

### A5


```csharp
public static PaperFormat A5 { get; }
```


A5: 5.8268 inches by 8.2677 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L69)

<a id="PuppeteerSharp_Media_PaperFormat_A6_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_A6"></a>

### A6


```csharp
public static PaperFormat A6 { get; }
```


A6: 4.1339 inches by 5.8268 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L74)

<a id="PuppeteerSharp_Media_PaperFormat_Height_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Height"></a>

### Height


```csharp
public decimal Height { get; set; }
```


Page height in inches.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The Height.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L86)

<a id="PuppeteerSharp_Media_PaperFormat_Ledger_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Ledger"></a>

### Ledger


```csharp
public static PaperFormat Ledger { get; }
```


Ledger: 17 inches by 11 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L39)

<a id="PuppeteerSharp_Media_PaperFormat_Legal_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Legal"></a>

### Legal


```csharp
public static PaperFormat Legal { get; }
```


Legal: 8.5 inches by 14 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L29)

<a id="PuppeteerSharp_Media_PaperFormat_Letter_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Letter"></a>

### Letter


```csharp
public static PaperFormat Letter { get; }
```


Letter: 8.5 inches x 11 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L24)

<a id="PuppeteerSharp_Media_PaperFormat_Tabloid_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Tabloid"></a>

### Tabloid


```csharp
public static PaperFormat Tabloid { get; }
```


Tabloid: 11 inches by 17 inches.

**Value:** [PaperFormat](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L34)

<a id="PuppeteerSharp_Media_PaperFormat_Width_"></a>
<a id="PuppeteerSharp_Media_PaperFormat_Width"></a>

### Width


```csharp
public decimal Width { get; set; }
```


Page width in inches.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal) — The width.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/PaperFormat.cs#L80)
