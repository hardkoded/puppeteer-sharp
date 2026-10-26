---
title: "JSCoverageEntry"
description: "The CoverageEntry class for JavaScript."
editUrl: false
prev: false
next: false
sidebar:
  label: "JSCoverageEntry"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_JSCoverageEntry"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record JSCoverageEntry : CoverageEntry, IEquatable<CoverageEntry>, IEquatable<JSCoverageEntry>
```


The CoverageEntry class for JavaScript.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object) → [CoverageEntry](/api/PuppeteerSharp.PageCoverage/CoverageEntry/)

**Implements:** [IEquatable&lt;CoverageEntry&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1), [IEquatable&lt;JSCoverageEntry&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/JSCoverageEntry.cs#L28)

## Properties

<a id="PuppeteerSharp_PageCoverage_JSCoverageEntry_RawScriptCoverage_"></a>
<a id="PuppeteerSharp_PageCoverage_JSCoverageEntry_RawScriptCoverage"></a>

### RawScriptCoverage


```csharp
public ScriptCoverage RawScriptCoverage { get; set; }
```


Raw V8 script coverage entry.

**Value:** [ScriptCoverage](/api/PuppeteerSharp.PageCoverage/ScriptCoverage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/JSCoverageEntry.cs#L33)
