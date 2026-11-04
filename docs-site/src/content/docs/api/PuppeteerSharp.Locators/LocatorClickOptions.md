---
title: "LocatorClickOptions"
description: "Options for locator click actions."
editUrl: false
prev: false
next: false
sidebar:
  label: "LocatorClickOptions"
  hidden: true
---

<a id="PuppeteerSharp_Locators_LocatorClickOptions"></a>

Namespace: [PuppeteerSharp.Locators](/api/PuppeteerSharp.Locators/)


```csharp
public class LocatorClickOptions
```


Options for locator click actions.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L9)

## Properties

<a id="PuppeteerSharp_Locators_LocatorClickOptions_Button_"></a>
<a id="PuppeteerSharp_Locators_LocatorClickOptions_Button"></a>

### Button


```csharp
public MouseButton Button { get; set; }
```


Gets or sets the mouse button to use for clicking.

**Value:** [MouseButton](/api/PuppeteerSharp.Input/MouseButton/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L19)

<a id="PuppeteerSharp_Locators_LocatorClickOptions_CancellationToken_"></a>
<a id="PuppeteerSharp_Locators_LocatorClickOptions_CancellationToken"></a>

### CancellationToken


```csharp
public CancellationToken CancellationToken { get; set; }
```


Gets or sets the cancellation token to cancel the action.

**Value:** [CancellationToken](https://learn.microsoft.com/dotnet/api/system.threading.cancellationtoken)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L14)

<a id="PuppeteerSharp_Locators_LocatorClickOptions_Count_"></a>
<a id="PuppeteerSharp_Locators_LocatorClickOptions_Count"></a>

### Count


```csharp
public int Count { get; set; }
```


Gets or sets the number of clicks.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L24)

<a id="PuppeteerSharp_Locators_LocatorClickOptions_Delay_"></a>
<a id="PuppeteerSharp_Locators_LocatorClickOptions_Delay"></a>

### Delay


```csharp
public int Delay { get; set; }
```


Gets or sets the time to wait between mousedown and mouseup in milliseconds.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L29)

<a id="PuppeteerSharp_Locators_LocatorClickOptions_OffSet_"></a>
<a id="PuppeteerSharp_Locators_LocatorClickOptions_OffSet"></a>

### OffSet


```csharp
public Offset? OffSet { get; set; }
```


Gets or sets the offset for the click relative to the top-left corner of the element's padding box.

**Value:** `Offset?`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/Locators/LocatorClickOptions.cs#L34)
