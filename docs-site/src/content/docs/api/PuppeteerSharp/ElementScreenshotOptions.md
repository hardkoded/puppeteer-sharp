---
title: "ElementScreenshotOptions"
description: "Options to be used in ElementScreenshotOptions), ScreenshotStreamAsync(ElementScreenshotOptions) and ScreenshotDataAsync(ElementScreenshotOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "ElementScreenshotOptions"
  hidden: true
---

<a id="PuppeteerSharp_ElementScreenshotOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class ElementScreenshotOptions : ScreenshotOptions
```


Options to be used in `ElementScreenshotOptions)`, [ScreenshotStreamAsync(ElementScreenshotOptions)](/api/PuppeteerSharp/IElementHandle/#PuppeteerSharp_IElementHandle_ScreenshotStreamAsync_PuppeteerSharp_ElementScreenshotOptions_) and [ScreenshotDataAsync(ElementScreenshotOptions)](/api/PuppeteerSharp/IElementHandle/#PuppeteerSharp_IElementHandle_ScreenshotDataAsync_PuppeteerSharp_ElementScreenshotOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [ScreenshotOptions](/api/PuppeteerSharp/ScreenshotOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementScreenshotOptions.cs#L6)

## Properties

<a id="PuppeteerSharp_ElementScreenshotOptions_ScrollIntoView_"></a>
<a id="PuppeteerSharp_ElementScreenshotOptions_ScrollIntoView"></a>

### ScrollIntoView


```csharp
public bool ScrollIntoView { get; set; }
```


When `true`, it will scroll into view before taking the screenshot. Defaults to `true`.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/ElementScreenshotOptions.cs#L11)
