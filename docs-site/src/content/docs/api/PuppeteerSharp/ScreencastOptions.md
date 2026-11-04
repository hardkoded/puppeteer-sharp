---
title: "ScreencastOptions"
description: "Options for ScreencastAsync(ScreencastOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ScreencastOptions"
  hidden: true
---

<a id="PuppeteerSharp_ScreencastOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ScreencastOptions
```


Options for [ScreencastAsync(ScreencastOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreencastAsync_PuppeteerSharp_ScreencastOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_ScreencastOptions_Colors_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Colors"></a>

### Colors


```csharp
public int? Colors { get; set; }
```


Specifies the maximum number of palette colors to quantize.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L66)

<a id="PuppeteerSharp_ScreencastOptions_Crop_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Crop"></a>

### Crop


```csharp
public BoundingBox Crop { get; set; }
```


Specifies the region of the viewport to crop.

**Value:** [BoundingBox](/api/PuppeteerSharp/BoundingBox/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L27)

<a id="PuppeteerSharp_ScreencastOptions_Delay_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Delay"></a>

### Delay


```csharp
public int? Delay { get; set; }
```


Specifies the delay between iterations of a loop, in ms.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L56)

<a id="PuppeteerSharp_ScreencastOptions_FfmpegPath_"></a>
<a id="PuppeteerSharp_ScreencastOptions_FfmpegPath"></a>

### FfmpegPath


```csharp
public string FfmpegPath { get; set; }
```


Path to the ffmpeg executable.
Required if ffmpeg is not in your PATH.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L72)

<a id="PuppeteerSharp_ScreencastOptions_Format_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Format"></a>

### Format


```csharp
public string Format { get; set; }
```


Specifies the output video format.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L22)

<a id="PuppeteerSharp_ScreencastOptions_Fps_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Fps"></a>

### Fps


```csharp
public int? Fps { get; set; }
```


Specifies the frame rate in frames per second.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L46)

<a id="PuppeteerSharp_ScreencastOptions_Loop_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Loop"></a>

### Loop


```csharp
public double? Loop { get; set; }
```


Specifies the number of times to loop playback.

**Value:** [double?](https://learn.microsoft.com/dotnet/api/system.double)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L51)

<a id="PuppeteerSharp_ScreencastOptions_Overwrite_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Overwrite"></a>

### Overwrite


```csharp
public bool? Overwrite { get; set; }
```


Specifies whether to overwrite the output file if it already exists.
Defaults to `true`. When `false`, an existing file causes an [IOException](https://learn.microsoft.com/dotnet/api/system.io.ioexception).

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L17)

<a id="PuppeteerSharp_ScreencastOptions_Path_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Path"></a>

### Path


```csharp
public string Path { get; set; }
```


File path to save the screencast to.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L11)

<a id="PuppeteerSharp_ScreencastOptions_Quality_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Quality"></a>

### Quality


```csharp
public int? Quality { get; set; }
```


Specifies the recording quality (Constant Rate Factor between 0-63).

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L61)

<a id="PuppeteerSharp_ScreencastOptions_Scale_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Scale"></a>

### Scale


```csharp
public decimal? Scale { get; set; }
```


Scales the output video.
For example, 0.5 will shrink the width and height of the output video by half.
2 will double the width and height of the output video.

**Value:** [decimal?](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L34)

<a id="PuppeteerSharp_ScreencastOptions_Speed_"></a>
<a id="PuppeteerSharp_ScreencastOptions_Speed"></a>

### Speed


```csharp
public decimal? Speed { get; set; }
```


Specifies the speed to record at.
For example, 0.5 will slowdown the output video by 50%.
2 will double the speed of the output video.

**Value:** [decimal?](https://learn.microsoft.com/dotnet/api/system.decimal)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreencastOptions.cs#L41)
