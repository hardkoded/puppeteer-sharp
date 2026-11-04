---
title: "FunctionCoverage"
description: "Coverage data for a JavaScript function."
editUrl: false
prev: false
next: false
sidebar:
  label: "FunctionCoverage"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_FunctionCoverage"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record FunctionCoverage : IEquatable<FunctionCoverage>
```


Coverage data for a JavaScript function.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;FunctionCoverage&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/FunctionCoverage.cs#L6)

## Properties

<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_FunctionName_"></a>
<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_FunctionName"></a>

### FunctionName


```csharp
public string FunctionName { get; set; }
```


JavaScript function name.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/FunctionCoverage.cs#L11)

<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_IsBlockCoverage_"></a>
<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_IsBlockCoverage"></a>

### IsBlockCoverage


```csharp
public bool IsBlockCoverage { get; set; }
```


Whether coverage data for this function has block granularity.

**Value:** [bool](https://learn.microsoft.com/dotnet/api/system.boolean)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/FunctionCoverage.cs#L21)

<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_Ranges_"></a>
<a id="PuppeteerSharp_PageCoverage_FunctionCoverage_Ranges"></a>

### Ranges


```csharp
public CoverageRange[] Ranges { get; set; }
```


Source ranges inside the function with coverage data.

**Value:** `CoverageRange\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/FunctionCoverage.cs#L16)
