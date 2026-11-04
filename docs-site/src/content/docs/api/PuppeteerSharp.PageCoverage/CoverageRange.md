---
title: "CoverageRange"
description: "Coverage data for a source range."
editUrl: false
prev: false
next: false
sidebar:
  label: "CoverageRange"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_CoverageRange"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record CoverageRange : IEquatable<CoverageRange>
```


Coverage data for a source range.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;CoverageRange&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageRange.cs#L6)

## Properties

<a id="PuppeteerSharp_PageCoverage_CoverageRange_Count_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageRange_Count"></a>

### Count


```csharp
public int Count { get; set; }
```


Collected execution count of the source range.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageRange.cs#L21)

<a id="PuppeteerSharp_PageCoverage_CoverageRange_EndOffset_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageRange_EndOffset"></a>

### EndOffset


```csharp
public int EndOffset { get; set; }
```


JavaScript script source offset for the range end.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageRange.cs#L16)

<a id="PuppeteerSharp_PageCoverage_CoverageRange_StartOffset_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageRange_StartOffset"></a>

### StartOffset


```csharp
public int StartOffset { get; set; }
```


JavaScript script source offset for the range start.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageRange.cs#L11)
