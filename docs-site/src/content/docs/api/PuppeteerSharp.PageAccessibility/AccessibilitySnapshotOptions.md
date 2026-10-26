---
title: "AccessibilitySnapshotOptions"
description: "SnapshotAsync(AccessibilitySnapshotOptions)."
editUrl: false
prev: false
next: false
sidebar:
  label: "AccessibilitySnapshotOptions"
  hidden: true
---

<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions"></a>

Namespace: [PuppeteerSharp.PageAccessibility](/api/PuppeteerSharp.PageAccessibility/)


```csharp
public class AccessibilitySnapshotOptions
```


[SnapshotAsync(AccessibilitySnapshotOptions)](/api/PuppeteerSharp.PageAccessibility/IAccessibility/#PuppeteerSharp_PageAccessibility_IAccessibility_SnapshotAsync_PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_).

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/AccessibilitySnapshotOptions.cs#L7)

## Properties

<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_IncludeIframes_"></a>
<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_IncludeIframes"></a>

### IncludeIframes


```csharp
public bool IncludeIframes { get; set; }
```


The root DOM element for the snapshot. Defaults to the whole page.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/AccessibilitySnapshotOptions.cs#L21)

<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_InterestingOnly_"></a>
<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_InterestingOnly"></a>

### InterestingOnly


```csharp
public bool InterestingOnly { get; set; }
```


Prune uninteresting nodes from the tree. Defaults to true.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/AccessibilitySnapshotOptions.cs#L12)

<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_Root_"></a>
<a id="PuppeteerSharp_PageAccessibility_AccessibilitySnapshotOptions_Root"></a>

### Root


```csharp
public IElementHandle Root { get; set; }
```


The root DOM element for the snapshot. Defaults to the whole page.

**Value:** [IElementHandle](/api/PuppeteerSharp/IElementHandle/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageAccessibility/AccessibilitySnapshotOptions.cs#L26)
