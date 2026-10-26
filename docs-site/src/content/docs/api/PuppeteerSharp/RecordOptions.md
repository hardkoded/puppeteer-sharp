---
title: "RecordOptions"
description: "Options for RecordAsync(RecordOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "RecordOptions"
  hidden: true
---

<a id="PuppeteerSharp_RecordOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class RecordOptions
```


Options for [RecordAsync(RecordOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_RecordAsync_PuppeteerSharp_RecordOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_RecordOptions_Audio_"></a>
<a id="PuppeteerSharp_RecordOptions_Audio"></a>

### Audio


```csharp
public bool? Audio { get; set; }
```


Whether to record audio.
Defaults to `false`.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L23)

<a id="PuppeteerSharp_RecordOptions_Fps_"></a>
<a id="PuppeteerSharp_RecordOptions_Fps"></a>

### Fps


```csharp
public int? Fps { get; set; }
```


Frame rate in frames per second (alias for [FrameRate](#PuppeteerSharp_RecordOptions_FrameRate)).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L43)

<a id="PuppeteerSharp_RecordOptions_FrameRate_"></a>
<a id="PuppeteerSharp_RecordOptions_FrameRate"></a>

### FrameRate


```csharp
public int? FrameRate { get; set; }
```


Maximum frame rate in frames per second.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L38)

<a id="PuppeteerSharp_RecordOptions_MaxHeight_"></a>
<a id="PuppeteerSharp_RecordOptions_MaxHeight"></a>

### MaxHeight


```csharp
public int? MaxHeight { get; set; }
```


Maximum frame height in pixels.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L33)

<a id="PuppeteerSharp_RecordOptions_MaxWidth_"></a>
<a id="PuppeteerSharp_RecordOptions_MaxWidth"></a>

### MaxWidth


```csharp
public int? MaxWidth { get; set; }
```


Maximum frame width in pixels.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L28)

<a id="PuppeteerSharp_RecordOptions_Overwrite_"></a>
<a id="PuppeteerSharp_RecordOptions_Overwrite"></a>

### Overwrite


```csharp
public bool? Overwrite { get; set; }
```


Specifies whether to overwrite the output file if it already exists.
Defaults to `true`. When `false`, an existing file causes an [IOException](https://learn.microsoft.com/dotnet/api/system.io.ioexception).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L17)

<a id="PuppeteerSharp_RecordOptions_Path_"></a>
<a id="PuppeteerSharp_RecordOptions_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


File path to save the recording to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/RecordOptions.cs#L11)
