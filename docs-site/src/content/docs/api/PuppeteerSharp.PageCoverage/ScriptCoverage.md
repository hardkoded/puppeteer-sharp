---
title: "ScriptCoverage"
description: "Coverage data for a JavaScript script."
editUrl: false
prev: false
next: false
sidebar:
  label: "ScriptCoverage"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_ScriptCoverage"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public record ScriptCoverage : IEquatable<ScriptCoverage>
```


Coverage data for a JavaScript script.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [IEquatable&lt;ScriptCoverage&gt;](https://learn.microsoft.com/dotnet/api/system.iequatable-1)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ScriptCoverage.cs#L6)

## Properties

<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_Functions_"></a>
<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_Functions"></a>

### Functions


```csharp
public FunctionCoverage[] Functions { get; set; }
```


Functions contained in the script that has coverage data.

**Value:** `FunctionCoverage\[\]`

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ScriptCoverage.cs#L21)

<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_ScriptId_"></a>
<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_ScriptId"></a>

### ScriptId


```csharp
public string ScriptId { get; set; }
```


JavaScript script id.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ScriptCoverage.cs#L11)

<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_Url_"></a>
<a id="PuppeteerSharp_PageCoverage_ScriptCoverage_Url"></a>

### Url


```csharp
public string Url { get; set; }
```


JavaScript script name or url.

**Value:** [string](https://learn.microsoft.com/dotnet/api/system.string)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/ScriptCoverage.cs#L16)
