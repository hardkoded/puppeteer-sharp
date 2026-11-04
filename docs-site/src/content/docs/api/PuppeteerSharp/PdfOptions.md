---
title: "PdfOptions"
description: "Options to be used in PdfOptions), PdfStreamAsync(PdfOptions) and PdfDataAsync(PdfOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "PdfOptions"
  hidden: true
---

<a id="PuppeteerSharp_PdfOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record PdfOptions : IEquatable<PdfOptions>
```


Options to be used in `PdfOptions)`, [PdfStreamAsync(PdfOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_PdfStreamAsync_PuppeteerSharp_PdfOptions_) and [PdfDataAsync(PdfOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_PdfDataAsync_PuppeteerSharp_PdfOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;PdfOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L10)

## Constructors

<a id="PuppeteerSharp_PdfOptions__ctor_"></a>
<a id="PuppeteerSharp_PdfOptions__ctor"></a>

### PdfOptions()


```csharp
public PdfOptions()
```


Initializes a new instance of the [PdfOptions](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L15)

## Properties

<a id="PuppeteerSharp_PdfOptions_DisplayHeaderFooter_"></a>
<a id="PuppeteerSharp_PdfOptions_DisplayHeaderFooter"></a>

### DisplayHeaderFooter


```csharp
public bool DisplayHeaderFooter { get; set; }
```


Display header and footer. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L27)

<a id="PuppeteerSharp_PdfOptions_FooterTemplate_"></a>
<a id="PuppeteerSharp_PdfOptions_FooterTemplate"></a>

### FooterTemplate


```csharp
public string FooterTemplate { get; set; }
```


HTML template for the print footer. Should be valid HTML markup with following classes used to inject printing values into them:
  `date` - formatted print date
  `title` - document title
  `url` - document location
  `pageNumber` - current page number
  `totalPages` - total pages in the document.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L47)

<a id="PuppeteerSharp_PdfOptions_Format_"></a>
<a id="PuppeteerSharp_PdfOptions_Format"></a>

### Format


```csharp
public PaperFormat Format { get; set; }
```


Paper format. If set, takes priority over [Width](#PuppeteerSharp_PdfOptions_Width) and [Height](#PuppeteerSharp_PdfOptions_Height).

**Value:** [PaperFormat](/api/PuppeteerSharp.Media/PaperFormat/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L67)

<a id="PuppeteerSharp_PdfOptions_HeaderTemplate_"></a>
<a id="PuppeteerSharp_PdfOptions_HeaderTemplate"></a>

### HeaderTemplate


```csharp
public string HeaderTemplate { get; set; }
```


HTML template for the print header. Should be valid HTML markup with following classes used to inject printing values into them:
  `date` - formatted print date
  `title` - document title
  `url` - document location
  `pageNumber` - current page number
  `totalPages` - total pages in the document.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L37)

<a id="PuppeteerSharp_PdfOptions_Height_"></a>
<a id="PuppeteerSharp_PdfOptions_Height"></a>

### Height


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Height { get; set; }
```


Paper height, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L78)

<a id="PuppeteerSharp_PdfOptions_Landscape_"></a>
<a id="PuppeteerSharp_PdfOptions_Landscape"></a>

### Landscape


```csharp
public bool Landscape { get; set; }
```


Paper orientation.. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L57)

<a id="PuppeteerSharp_PdfOptions_MarginOptions_"></a>
<a id="PuppeteerSharp_PdfOptions_MarginOptions"></a>

### MarginOptions


```csharp
public MarginOptions MarginOptions { get; set; }
```


Paper margins, defaults to none.

**Value:** [MarginOptions](/api/PuppeteerSharp.Media/MarginOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L84)

<a id="PuppeteerSharp_PdfOptions_OmitBackground_"></a>
<a id="PuppeteerSharp_PdfOptions_OmitBackground"></a>

### OmitBackground


```csharp
public bool OmitBackground { get; set; }
```


Hides default white background and allows generating pdfs with transparency.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L95)

<a id="PuppeteerSharp_PdfOptions_Outline_"></a>
<a id="PuppeteerSharp_PdfOptions_Outline"></a>

### Outline


```csharp
public bool Outline { get; set; }
```


Generate document outline.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

If this is enabled the PDF will also be tagged (accessible)
Currently only works in old Headless (headless = true).

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L109)

<a id="PuppeteerSharp_PdfOptions_PageRanges_"></a>
<a id="PuppeteerSharp_PdfOptions_PageRanges"></a>

### PageRanges


```csharp
public string PageRanges { get; set; }
```


Paper ranges to print, e.g., `1-5, 8, 11-13`. Defaults to the empty string, which means print all pages.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L62)

<a id="PuppeteerSharp_PdfOptions_PreferCSSPageSize_"></a>
<a id="PuppeteerSharp_PdfOptions_PreferCSSPageSize"></a>

### PreferCSSPageSize


```csharp
public bool PreferCSSPageSize { get; set; }
```


Give any CSS `@page` size declared in the page priority over what is declared in `width` and `height` or `format` options.
Defaults to `false`, which will scale the content to fit the paper size.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L90)

<a id="PuppeteerSharp_PdfOptions_PrintBackground_"></a>
<a id="PuppeteerSharp_PdfOptions_PrintBackground"></a>

### PrintBackground


```csharp
public bool PrintBackground { get; set; }
```


Print background graphics. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L52)

<a id="PuppeteerSharp_PdfOptions_Scale_"></a>
<a id="PuppeteerSharp_PdfOptions_Scale"></a>

### Scale


```csharp
public decimal Scale { get; set; }
```


Scale of the webpage rendering. Defaults to `1`. Scale amount must be between 0.1 and 2.

**Value:** [decimal](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L22)

<a id="PuppeteerSharp_PdfOptions_Tagged_"></a>
<a id="PuppeteerSharp_PdfOptions_Tagged"></a>

### Tagged


```csharp
public bool Tagged { get; set; }
```


Generate tagged (accessible) PDF.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L100)

<a id="PuppeteerSharp_PdfOptions_Timeout_"></a>
<a id="PuppeteerSharp_PdfOptions_Timeout"></a>

### Timeout


```csharp
[JsonIgnore]
public int? Timeout { get; set; }
```


Timeout in milliseconds. Pass `0` to disable timeout.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

The default value can be changed by using the [DefaultTimeout](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_DefaultTimeout) property.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L125)

<a id="PuppeteerSharp_PdfOptions_WaitForFonts_"></a>
<a id="PuppeteerSharp_PdfOptions_WaitForFonts"></a>

### WaitForFonts


```csharp
public bool WaitForFonts { get; set; }
```


If `true`, waits for `document.fonts.ready` to resolve. This might require
activating the page using [BringToFrontAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_BringToFrontAsync) if the page is in the
background.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

Defaults to `true`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L117)

<a id="PuppeteerSharp_PdfOptions_Width_"></a>
<a id="PuppeteerSharp_PdfOptions_Width"></a>

### Width


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Width { get; set; }
```


Paper width, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PdfOptions.cs#L72)
