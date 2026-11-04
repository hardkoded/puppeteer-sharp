---
title: "WaitForSelectorOptions"
description: "Optional waiting parameters."
editUrl: false
prev: false
next: false
sidebar:
  label: "WaitForSelectorOptions"
  hidden: true
---

<a id="PuppeteerSharp_WaitForSelectorOptions"></a>

Namespace: [PuppeteerSharp](/api/PuppeteerSharp/)


```csharp
public class WaitForSelectorOptions : WaitForOptions
```


Optional waiting parameters.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [WaitForOptions](/api/PuppeteerSharp/WaitForOptions/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForSelectorOptions.cs#L8)

## Properties

<a id="PuppeteerSharp_WaitForSelectorOptions_Hidden_"></a>
<a id="PuppeteerSharp_WaitForSelectorOptions_Hidden"></a>

### Hidden


```csharp
public bool? Hidden { get; set; }
```


Wait for element to not be found in the DOM or to be hidden.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForSelectorOptions.cs#L18)

<a id="PuppeteerSharp_WaitForSelectorOptions_Root_"></a>
<a id="PuppeteerSharp_WaitForSelectorOptions_Root"></a>

### Root


```csharp
public IElementHandle Root { get; set; }
```


Root element.

**Value:** [IElementHandle](/api/PuppeteerSharp/IElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForSelectorOptions.cs#L23)

<a id="PuppeteerSharp_WaitForSelectorOptions_Visible_"></a>
<a id="PuppeteerSharp_WaitForSelectorOptions_Visible"></a>

### Visible


```csharp
public bool? Visible { get; set; }
```


Wait for element to be present in DOM and to be visible.

**Value:** [bool?](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/WaitForSelectorOptions.cs#L13)
