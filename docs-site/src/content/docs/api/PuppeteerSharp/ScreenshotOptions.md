---
title: "ScreenshotOptions"
description: "Options to be used in ScreenshotOptions), ScreenshotStreamAsync(ScreenshotOptions) and ScreenshotDataAsync(ScreenshotOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ScreenshotOptions"
  hidden: true
---

<a id="PuppeteerSharp_ScreenshotOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ScreenshotOptions
```


Options to be used in `ScreenshotOptions)`, [ScreenshotStreamAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotStreamAsync_PuppeteerSharp_ScreenshotOptions_) and [ScreenshotDataAsync(ScreenshotOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_ScreenshotDataAsync_PuppeteerSharp_ScreenshotOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Derived:** [ElementScreenshotOptions](/api/PuppeteerSharp/ElementScreenshotOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L10)

## Properties

<a id="PuppeteerSharp_ScreenshotOptions_BurstMode_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_BurstMode"></a>

### BurstMode


```csharp
[JsonIgnore]
public bool BurstMode { get; set; }
```


When BurstMode is `true` the screenshot process will only execute all the screenshot setup actions (background and metrics overrides)
before the first screenshot call and it will ignore the reset actions after the screenshot is taken.
[SetBurstModeOffAsync()](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetBurstModeOffAsync) needs to be called after the last screenshot is taken.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

**Example**

var screenShotOptions = new ScreenshotOptions
{
    FullPage = true,
    BurstMode = true
};
await page.GoToAsync("https://www.google.com");
for(var x = 0; x &lt; 100; x++)
{
    await page.ScreenshotBase64Async(screenShotOptions);
}
await page.SetBurstModeOffAsync();

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L60)

<a id="PuppeteerSharp_ScreenshotOptions_CaptureBeyondViewport_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_CaptureBeyondViewport"></a>

### CaptureBeyondViewport


```csharp
public bool CaptureBeyondViewport { get; set; }
```


Capture the screenshot beyond the viewport.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L66)

<a id="PuppeteerSharp_ScreenshotOptions_Clip_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_Clip"></a>

### Clip


```csharp
public Clip Clip { get; set; }
```


Specifies clipping region of the page/element.

**Value:** [Clip](/api/PuppeteerSharp.Media/Clip/) — The clip.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L16)

<a id="PuppeteerSharp_ScreenshotOptions_FromSurface_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_FromSurface"></a>

### FromSurface


```csharp
public bool? FromSurface { get; set; }
```


Capture the screenshot from the surface, rather than the view. Defaults to `true`.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L71)

<a id="PuppeteerSharp_ScreenshotOptions_FullPage_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_FullPage"></a>

### FullPage


```csharp
public bool FullPage { get; set; }
```


When `true`, takes a screenshot of the full scrollable page. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if full page; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L22)

<a id="PuppeteerSharp_ScreenshotOptions_OmitBackground_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_OmitBackground"></a>

### OmitBackground


```csharp
public bool OmitBackground { get; set; }
```


Hides default white background and allows capturing screenshots with transparency. Defaults to `false`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — `true` if omit background; otherwise, `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L28)

<a id="PuppeteerSharp_ScreenshotOptions_OptimizeForSpeed_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_OptimizeForSpeed"></a>

### OptimizeForSpeed


```csharp
public bool? OptimizeForSpeed { get; set; }
```


Optimize image encoding for speed.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L76)

<a id="PuppeteerSharp_ScreenshotOptions_Quality_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_Quality"></a>

### Quality


```csharp
public int? Quality { get; set; }
```


The quality of the image, between 0-100. Not applicable to png images.

**Value:** [int?](https://learn.microsoft.com/dotnet/api/system.int32) — The quality.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L40)

<a id="PuppeteerSharp_ScreenshotOptions_Type_"></a>
<a id="PuppeteerSharp_ScreenshotOptions_Type"></a>

### Type


```csharp
public ScreenshotType? Type { get; set; }
```


Specify screenshot type, can be either jpeg, png or webp. Defaults to 'png'.

**Value:** `ScreenshotType?` — The type.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ScreenshotOptions.cs#L34)
