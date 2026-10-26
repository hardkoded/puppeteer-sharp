---
title: "ViewPortOptions"
description: "View port options used on SetViewportAsync(ViewPortOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ViewPortOptions"
  hidden: true
---

<a id="PuppeteerSharp_ViewPortOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public record ViewPortOptions : IEquatable<ViewPortOptions>
```


View port options used on [SetViewportAsync(ViewPortOptions)](/api/PuppeteerSharp/IPage/#PuppeteerSharp_IPage_SetViewportAsync_PuppeteerSharp_ViewPortOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;ViewPortOptions&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_ViewPortOptions_Default_"></a>
<a id="PuppeteerSharp_ViewPortOptions_Default"></a>

### Default


```csharp
public static ViewPortOptions Default { get; }
```


Default Viewport.

**Value:** [ViewPortOptions](./)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L11)

<a id="PuppeteerSharp_ViewPortOptions_DeviceScaleFactor_"></a>
<a id="PuppeteerSharp_ViewPortOptions_DeviceScaleFactor"></a>

### DeviceScaleFactor


```csharp
public double DeviceScaleFactor { get; set; }
```


Gets or sets the device scale factor.

**Value:** [double](https://learn.microsoft.com/dotnet/api/system.double) — Specify device scale factor (can be thought of as dpr). Defaults to 1.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L39)

<a id="PuppeteerSharp_ViewPortOptions_HasTouch_"></a>
<a id="PuppeteerSharp_ViewPortOptions_HasTouch"></a>

### HasTouch


```csharp
public bool HasTouch { get; set; }
```


Gets or sets if viewport supports touch events.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Specifies if viewport supports touch events. Defaults to `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L51)

<a id="PuppeteerSharp_ViewPortOptions_Height_"></a>
<a id="PuppeteerSharp_ViewPortOptions_Height"></a>

### Height


```csharp
public int Height { get; set; }
```


Gets or sets the height.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — The page height in pixels.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L27)

<a id="PuppeteerSharp_ViewPortOptions_IsLandscape_"></a>
<a id="PuppeteerSharp_ViewPortOptions_IsLandscape"></a>

### IsLandscape


```csharp
public bool IsLandscape { get; set; }
```


Gets or sets if viewport is in landscape mode.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Specifies if viewport is in landscape mode. Defaults to `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L45)

<a id="PuppeteerSharp_ViewPortOptions_IsMobile_"></a>
<a id="PuppeteerSharp_ViewPortOptions_IsMobile"></a>

### IsMobile


```csharp
public bool IsMobile { get; set; }
```


Gets or sets whether the meta viewport tag is taken into account.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean) — Whether the meta viewport tag is taken into account. Defaults to `false`.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L33)

<a id="PuppeteerSharp_ViewPortOptions_Width_"></a>
<a id="PuppeteerSharp_ViewPortOptions_Width"></a>

### Width


```csharp
public int Width { get; set; }
```


Gets or sets the width.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — The page width in pixels.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ViewPortOptions.cs#L21)
