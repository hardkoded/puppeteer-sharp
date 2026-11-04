---
title: "MarginOptions"
description: "margin options used in PdfOptions."
editUrl: false
prev: false
next: false
sidebar:
  label: "MarginOptions"
  hidden: true
---

<a id="PuppeteerSharp_Media_MarginOptions"></a>

Namespace: [PuppeteerSharp.Media](/api/PuppeteerSharp.Media/)


```csharp
public record MarginOptions : IEquatable<MarginOptions>
```


margin options used in [PdfOptions](/api/PuppeteerSharp/PdfOptions/).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;MarginOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L9)

## Constructors

<a id="PuppeteerSharp_Media_MarginOptions__ctor_"></a>
<a id="PuppeteerSharp_Media_MarginOptions__ctor"></a>

### MarginOptions()


```csharp
public MarginOptions()
```


Initializes a new instance of the [MarginOptions](./) class.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L14)

## Properties

<a id="PuppeteerSharp_Media_MarginOptions_Bottom_"></a>
<a id="PuppeteerSharp_Media_MarginOptions_Bottom"></a>

### Bottom


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Bottom { get; set; }
```


Bottom margin, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L33)

<a id="PuppeteerSharp_Media_MarginOptions_Left_"></a>
<a id="PuppeteerSharp_Media_MarginOptions_Left"></a>

### Left


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Left { get; set; }
```


Left margin, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L27)

<a id="PuppeteerSharp_Media_MarginOptions_Right_"></a>
<a id="PuppeteerSharp_Media_MarginOptions_Right"></a>

### Right


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Right { get; set; }
```


Right margin, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L39)

<a id="PuppeteerSharp_Media_MarginOptions_Top_"></a>
<a id="PuppeteerSharp_Media_MarginOptions_Top"></a>

### Top


```csharp
[JsonConverter(typeof(PrimitiveTypeConverter))]
public object Top { get; set; }
```


Top margin, accepts values labeled with units.

**Value:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Media/MarginOptions.cs#L21)
