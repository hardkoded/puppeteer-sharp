---
title: "Coverage"
description: "Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it..."
editUrl: false
prev: false
next: false
sidebar:
  label: "Coverage"
  hidden: true
---

<a id="PuppeteerSharp_PageCoverage_Coverage"></a>

Namespace: [PuppeteerSharp.PageCoverage](/api/PuppeteerSharp.PageCoverage/)


```csharp
public class Coverage : ICoverage
```


Supports all classes in the .NET class hierarchy and provides low-level services to derived classes. This is the ultimate base class of all .NET classes; it is the root of the type hierarchy.

**Inheritance:** [object](https://learn.microsoft.com/dotnet/api/system.object)

**Implements:** [ICoverage](/api/PuppeteerSharp.PageCoverage/ICoverage/)

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/Coverage.cs#L8)

## Methods

<a id="PuppeteerSharp_PageCoverage_Coverage_StartCSSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_Coverage_StartCSSCoverageAsync_PuppeteerSharp_PageCoverage_CoverageStartOptions_"></a>

### StartCSSCoverageAsync(CoverageStartOptions)


```csharp
public Task StartCSSCoverageAsync(CoverageStartOptions options = null)
```


Starts CSS coverage.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CoverageStartOptions](/api/PuppeteerSharp.PageCoverage/CoverageStartOptions/) | Set of configurable options for coverage. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when coverage is started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/Coverage.cs#L27)

<a id="PuppeteerSharp_PageCoverage_Coverage_StartJSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_Coverage_StartJSCoverageAsync_PuppeteerSharp_PageCoverage_CoverageStartOptions_"></a>

### StartJSCoverageAsync(CoverageStartOptions)


```csharp
public Task StartJSCoverageAsync(CoverageStartOptions options = null)
```


Starts JS coverage.

**Parameters**

| Name | Type | Description |
| --- | --- | --- |
| `options` | [CoverageStartOptions](/api/PuppeteerSharp.PageCoverage/CoverageStartOptions/) | Set of configurable options for coverage defaults to resetOnNavigation : true, reportAnonymousScripts : false, includeRawScriptCoverage : false, useBlockCoverage : true. |

**Returns:** [Task](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task) — A task that resolves when coverage is started.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/Coverage.cs#L20)

<a id="PuppeteerSharp_PageCoverage_Coverage_StopCSSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_Coverage_StopCSSCoverageAsync"></a>

### StopCSSCoverageAsync()


```csharp
public Task<CoverageEntry[]> StopCSSCoverageAsync()
```


Stops JS coverage and returns coverage reports for all non-anonymous scripts.

**Returns:** [Task&lt;CoverageEntry\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the array of coverage reports for all stylesheets.

JavaScript Coverage doesn't include anonymous scripts; however, scripts with sourceURLs are reported.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/Coverage.cs#L31)

<a id="PuppeteerSharp_PageCoverage_Coverage_StopJSCoverageAsync_"></a>
<a id="PuppeteerSharp_PageCoverage_Coverage_StopJSCoverageAsync"></a>

### StopJSCoverageAsync()


```csharp
public Task<JSCoverageEntry[]> StopJSCoverageAsync()
```


Stops JS coverage and returns coverage reports for all scripts.

**Returns:** [Task&lt;JSCoverageEntry\[\]&gt;](https://learn.microsoft.com/dotnet/api/system.threading.tasks.task-1) — Task that resolves to the array of coverage reports for all stylesheets.

JavaScript Coverage doesn't include anonymous scripts by default; however, scripts with sourceURLs are reported.

[View source](https://github.com/hardkoded/puppeteer-sharp/blob/master/lib/PuppeteerSharp/PageCoverage/Coverage.cs#L24)
