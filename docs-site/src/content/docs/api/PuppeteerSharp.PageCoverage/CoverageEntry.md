---
title: "CoverageEntry"
description: "Coverage report for all non-anonymous scripts."
editUrl: false
prev: false
next: false
sidebar:
  label: "CoverageEntry"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_CoverageEntry"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record CoverageEntry : IEquatable<CoverageEntry>
```


Coverage report for all non-anonymous scripts.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;CoverageEntry&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

**Derived:** [JSCoverageEntry](/api/PuppeteerSharp.PageCoverage/JSCoverageEntry/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntry.cs#L6)

## Properties

<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Ranges_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Ranges"></a>

### Ranges


```csharp
public CoverageEntryRange[] Ranges { get; set; }
```


Script ranges that were executed. Ranges are sorted and non-overlapping.

**Value:** `CoverageEntryRange\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntry.cs#L16)

<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Text_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Text"></a>

### Text


```csharp
public string Text { get; set; }
```


Script content.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntry.cs#L21)

<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Url_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageEntry_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


Script URL.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntry.cs#L11)
