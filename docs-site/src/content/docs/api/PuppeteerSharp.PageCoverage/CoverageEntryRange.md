---
title: "CoverageEntryRange"
description: "Script range."
editUrl: false
prev: false
next: false
sidebar:
  label: "CoverageEntryRange"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_CoverageEntryRange"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record CoverageEntryRange : IEquatable<CoverageEntryRange>
```


Script range.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;CoverageEntryRange&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntryRange.cs#L6)

## Properties

<a id="PuppeteerSharp_PageCoverage_CoverageEntryRange_End_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageEntryRange_End"></a>

### End


```csharp
public int End { get; }
```


An end offset in text, exclusive.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — End offset.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntryRange.cs#L18)

<a id="PuppeteerSharp_PageCoverage_CoverageEntryRange_Start_"></a>
<a id="PuppeteerSharp_PageCoverage_CoverageEntryRange_Start"></a>

### Start


```csharp
public int Start { get; }
```


A start offset in text, inclusive.

**Value:** [int](https://learn.microsoft.com/dotnet/api/system.int32) — Start offset.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/CoverageEntryRange.cs#L12)
