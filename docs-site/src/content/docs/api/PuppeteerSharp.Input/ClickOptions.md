---
title: "ClickOptions"
description: "Options to use when clicking."
editUrl: false
prev: false
next: false
sidebar:
  label: "ClickOptions"
  hidden: true
---

<a id="PuppeteerSharp_Input_ClickOptions"></a>

Namespace: [PuppeteerSharp.Input](/api/PuppeteerSharp.Input/)


```csharp
public class ClickOptions
```


Options to use when clicking.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ClickOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_Input_ClickOptions_Button_"></a>
<a id="PuppeteerSharp_Input_ClickOptions_Button"></a>

### Button


```csharp
public MouseButton Button { get; set; }
```


The button to use for the click. Defaults to [Left](/api/PuppeteerSharp.Input/MouseButton/#PuppeteerSharp_Input_MouseButton_Left).

**Value:** [MouseButton](/api/PuppeteerSharp.Input/MouseButton/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ClickOptions.cs#L21)

<a id="PuppeteerSharp_Input_ClickOptions_Count_"></a>
<a id="PuppeteerSharp_Input_ClickOptions_Count"></a>

### Count


```csharp
public int Count { get; set; }
```


Defaults to 1. See https://developer.mozilla.org/en-US/docs/Web/API/UIEvent/detail.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ClickOptions.cs#L16)

<a id="PuppeteerSharp_Input_ClickOptions_Delay_"></a>
<a id="PuppeteerSharp_Input_ClickOptions_Delay"></a>

### Delay


```csharp
public int Delay { get; set; }
```


Time to wait between `mousedown` and `mouseup` in milliseconds. Defaults to 0.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ClickOptions.cs#L11)

<a id="PuppeteerSharp_Input_ClickOptions_OffSet_"></a>
<a id="PuppeteerSharp_Input_ClickOptions_OffSet"></a>

### OffSet


```csharp
public Offset? OffSet { get; set; }
```


Offset for the clickable point relative to the top-left corner of the border-box.

**Value:** `Offset?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Input/ClickOptions.cs#L26)
